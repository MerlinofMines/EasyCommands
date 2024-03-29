﻿# Contributing To EasyCommands

Try as I might, I'm not able to do everything which folks request including new features, fixing bugs, etc.  I only have so much time to dedicate to further improvements.

Thanksfully, there are plenty of folks able and hopefully willing to contribute to EasyCommands.  These instructions will serve as a guideline for contributing to EasyCommands.

## Ground Rules

### I Have the Final Say
I have the final say on whether a change or new feature will be added.  I'm a reasonable guy and I don't intend confrontation, but I will push back on unreasonable changes or functionality.

Before submitting a pull request you should open an [issue](https://github.com/MerlinofMines/EasyCommands/issues) describing the bug/feature request.  Please label it so that we can triage appropriately.  Please put enough detail into the issue so that we can have a good conversation about it before implementing.

Before pursuing a large change, let's chat and make sure we're aligned on approach.

### Size Matters
The script can only be 100,000 characters, so that folks can use it without modifying SE.  As of this writing, the character size is about 82,000, and that is after extensive work to reduce the character footprint.  As such, some features may literally not be worth the characters they cost to implement, especially if a reasonable workaround exists.

Additionally, this means that there are lots of patterns in place whose purpose is to reduce character footprint.   If you don't understand how/why the code works the way it does, just ask.  If you have ideas on how to refactor the code to reduce character footprint while maintaining (or improving) extensibility, I am all ears.

### Testing Matters

Before submitting a pull request, run all of the tests and make sure that they pass.  Also include tests for any modified or new functionality.  You should add tests on any modified or added behavior.  The tests aren't hard to write.  In fact, you usually just write a sample script using the behavior, and then verify the expectations using Mock (Moq) blocks.

The entire test suite is run on any pull request submission and any code commit to a branch (master or otherwise) to ensure all tests are passing for every pull request.

If you're changing functionality for real in-game blocks, you should test your script manually to make sure the behavior you think works actually works.  Often times the real game doesn't work quite like we think it does, even though the tests pass.

### Documentation Matters

A feature is no good if folks don't know about it.  There's a website dedicated to describing the behavior of EasyCommands, and it needs to be kept up to date.  If you are changing or adding behavior, please add documentation for it.  If you're unsure how/where to document, just ask.

Changes to documentation on the "master" branch will not affect the live site, so documentation changes should be included as part of the pull request for the feature itself.

### Be Patient, Be Polite, Expect Feedback
Expect some feedback on your pull request before having it approved.  Additionally, I only have so much time to review pull requests.  If it takes me a bit to reply, don't fret.  Politely nudge me on Steam, if needed.

## Pulling Down the Code

### Dependencies
Ok, now that the ground rules are out of the way, let's do this!  To pull down the code yourself, you're going to need a few things.

* [Git](https://git-scm.com/download/win)
* [Visual Studio](https://visualstudio.microsoft.com/downloads/)
* [MDK Visual Studio Extension](https://github.com/malware-dev/MDK-SE/wiki/Getting-Started)

### Getting EasyCommands Set Up Locally

Once you've got these dependencies installed, here's how you can import EasyCommands into Visual Studio.  Check out [this useful guide](https://opensource.com/article/19/7/create-pull-request-github) for information on standard practice for forking a repo and creating a pull request.

After you've forked, cloned, created your feature branch, and added remote, do the following to get set up in Visual Studio:

*  Open the existing EasyCommands/EasyCommands.sln file from Visual Studio

*  MDK should ask you to repair the EasyCommands project.  This will bind your local SpaceEngineers installation with EasyCommands so that you can deploy your script locally. Go ahead and click "Repair"

    ![Repair](https://imgur.com/KRSSs0y.png)

    ![Upgrade Complete](https://imgur.com/L0DfSpP.png)

    ![Valid Upgrade](https://imgur.com/INz7Xqe.png)

*  Build the Solution.  This will build EasyCommands and EasyCommands.Tests

    ![Build Solution](https://imgur.com/ouTAozx.png)

*  Right click EasyCommands.Tests in the Solution Explorer and click "Run Tests".  This will make sure that all of the EasyCommands tests are passing.

    ![Run Tests](https://imgur.com/C07aPpb.png)

    ![Tests Passing](https://imgur.com/p6943wz.png)

That's it! You should be ready to contribute to EasyCommands.

## Build on "master"

You should always base your changes off of the "master" branch, which is the "bleeding edge" including features that haven't been released yet but will be in the next release.

The documentation is built against the "currentRelease" branch.  Why?  Because you shouldn't tell folks about a feature that doesn't work yet!  If the live docs need to be fixed, submit a pull request against "master", and I can cherry-pick it onto currentRelease.  That way we don't forget it in the upcoming release.

When I release a new version of EasyCommands I will merge all changes from master into currentRelease which will update all of the docs appropriately and simultaneously, around the same time as the actual script is re-published to Steam.

## Before You Submit a Pull Request

* Open an [issue](https://github.com/MerlinofMines/EasyCommands/issues) and make sure we're aligned on plan to implement/fix.

* Make sure all tests are passing.  You should have new tests for any added functionality. 

* If your changes are visible to people using the script, add documentation for your changes/additions. 

*  If you're changing block behavior, please manually test in Space Engineers.  To do this:
    * [Install PBUnlimiter.dll from MDK](https://github.com/malware-dev/MDK-SE/tree/master/binaries)
    * Change the MDK Script Options Minifier to "Strip Comments" or "None" in the EasyCommands project menu.
    * Run MDK Deploy Script from EasyCommands project menu to deploy your modified script locally.
    * Load up Space Engineers, put EasyCommands into a PB, and make sure the functionality works as you intend.

*  Look for ways to condense the characters needed to implement your feature.