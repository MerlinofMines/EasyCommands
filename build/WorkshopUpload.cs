// Built locally against the Steamworks.NET shipped with Space Engineers.
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Steamworks;

internal static class WorkshopUpload
{
    private const ulong ProductionId = 2698315573;
    private static void Wait(Func<bool> complete)
    {
        var timer = Stopwatch.StartNew();
        while (!complete())
        {
            if (timer.Elapsed > TimeSpan.FromMinutes(5))
                throw new TimeoutException("Steam request timed out. Check the item before retrying.");
            SteamAPI.RunCallbacks();
            Thread.Sleep(50);
        }
    }

    private static SteamUGCDetails_t Query(ulong id)
    {
            bool queried = false;
            bool queryFailed = false;
            SteamUGCDetails_t details = new SteamUGCDetails_t();
            using (var query = CallResult<SteamUGCRequestUGCDetailsResult_t>.Create((result, failure) =>
            {
                details = result.m_details;
                queryFailed = failure;
                queried = true;
            }))
            {
                query.Set(SteamUGC.RequestUGCDetails((PublishedFileId_t)id, 0));
                Wait(() => queried);
            }
            if (queryFailed || details.m_eResult != EResult.k_EResultOK)
                throw new InvalidOperationException("Cannot query item: " + details.m_eResult);
            return details;
    }

    public static int Main(string[] args)
    {
        bool initialized = false;
        try
        {
            if (args.Length != 5) throw new ArgumentException("Expected: itemId contentFolder notesFile production|private-test Preserve|Private|Unlisted");
            ulong id = ulong.Parse(args[0]);
            bool privateTest = args[3] == "private-test";
            if (args[3] != "production" && !privateTest) throw new ArgumentException("Unknown publishing mode.");
            if (id == 0 || (privateTest ? id == ProductionId : id != ProductionId))
                throw new ArgumentException("Workshop ID is not allowed in this publishing mode.");
            if (privateTest ? (args[4] != "Private" && args[4] != "Unlisted") : args[4] != "Preserve") throw new ArgumentException("Invalid visibility for publishing mode.");
            string content = Path.GetFullPath(args[1]);
            string notes = File.ReadAllText(args[2], Encoding.UTF8);
            if (!File.Exists(Path.Combine(content, "script.cs"))) throw new ArgumentException("Missing script.cs.");
            if (string.IsNullOrWhiteSpace(notes) || notes.IndexOf('\0') >= 0)
                throw new ArgumentException("Notes must contain text and must not contain NUL characters.");
            Environment.SetEnvironmentVariable("SteamAppId", "244850");
            Environment.SetEnvironmentVariable("SteamGameId", "244850");
            initialized = SteamAPI.Init();
            if (!initialized) throw new InvalidOperationException("Cannot initialize Steam. Run Steam as the same Windows user and sign into the item's owner account.");
            if ((uint)SteamUtils.GetAppID() != 244850) throw new InvalidOperationException("Wrong Steam app ID.");
            var details = Query(id);
            if ((ulong)details.m_nPublishedFileId != id || (uint)details.m_nConsumerAppID != 244850 ||
                details.m_ulSteamIDOwner != (ulong)SteamUser.GetSteamID())
                throw new InvalidOperationException("Item ID, game, or signed-in Steam owner does not match.");
            if (privateTest && details.m_eVisibility != ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate && details.m_eVisibility != ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityUnlisted)
                throw new InvalidOperationException("Test item must already be private or unlisted.");
            Console.WriteLine("Verified Steam owner and item: " + id + "; " + details.m_rgchTitle);
            var update = SteamUGC.StartItemUpdate((AppId_t)244850, (PublishedFileId_t)id);
            if (privateTest && !SteamUGC.SetItemVisibility(update, args[4] == "Unlisted" ?
                ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityUnlisted :
                ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate))
                throw new InvalidOperationException("Steam rejected the requested test visibility.");
            if (!SteamUGC.SetItemContent(update, content)) throw new InvalidOperationException("Steam rejected content folder.");
            Console.WriteLine("Submitting release notes:\n" + notes);
            bool done = false;
            bool failed = false;
            SubmitItemUpdateResult_t response = new SubmitItemUpdateResult_t();
            using (var submit = CallResult<SubmitItemUpdateResult_t>.Create((result, failure) =>
            {
                response = result;
                failed = failure;
                done = true;
            }))
            {
                submit.Set(SteamUGC.SubmitItemUpdate(update, notes));
                Wait(() => done);
            }
            if (failed || response.m_eResult != EResult.k_EResultOK)
                throw new InvalidOperationException("Steam upload failed: " + response.m_eResult);
            if (response.m_bUserNeedsToAcceptWorkshopLegalAgreement)
                throw new InvalidOperationException("Steam requires the owner to accept its Workshop agreement. Check the item before retrying.");
            var expectedVisibility = privateTest ?
                (args[4] == "Unlisted" ? ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityUnlisted :
                 ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate) : details.m_eVisibility;
            var uploaded = Query(id);
            if (uploaded.m_eVisibility != expectedVisibility)
                throw new InvalidOperationException("Visibility verification failed: " + uploaded.m_eVisibility + "; expected " + expectedVisibility);
            Console.WriteLine("Verified final visibility: " + uploaded.m_eVisibility);
            Console.WriteLine("Steam accepted the content and notes. Verify the printed Change Notes URL in the browser.");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error.Message); return 1; }
        finally { if (initialized) SteamAPI.Shutdown(); }
    }
}

