﻿# EasyCommands Cheat Sheet
The following lists out all of the reserved keywords in EasyCommands.

All reserved keywords are matched by lowercase check, so capitalization does not matter.

It does not include [Items & Blueprints](https://spaceengineers.merlinofmines.com/EasyCommands/items "Items & Blueprints"), which are not actually reserved keywords.

## Ignored Keywords
These words are (mostly) ignored when parsing your script, but feel free to use them to make your commands sound more natural.

* ```the, than, turned, block, panel, chamber, drive, to, from, then, of, either, for, in, do, does, second, seconds, be, being, digit, digits```
* ```#``` - Script Comment, when used as the first character of a line (after trimming spaces).

## Selectors
* Named Selector - ```"``` (also used for strings)
* Conditional Selector - ```that, which, whose```
* Group Selector - ```blocks, group, panels, chambers, drives```
* Index Selector - ```@``` (note you can also use ```[]```)
* Self Selector - ```my, self, this``` 
* Variable Selector - ```$```

### Block Types (Single, Group) 
* Air Vent - ```airvent, vent```, ```airvents, vents```
* Antenna - ```antenna```, ```antennas```
* Assembler - ```assembler```, ```assemblers```
* Battery - ```battery```, ```batteries```
* Beacon - ```beacon```, ```beacons```
* Camera - ```camera```, ```cameras```
* Cockpit - ```cockpit, ship, rover, seat, station```, ```cockpits, ships, rovers, seats, stations```
* Collector - ```collector```, ```collectors```
* Connector - ```connector```, ```connectors```
* Cryo Chamber - ```cryo```, ```cryos```
* Decoy - ```decoy```, ```decoys```
* Display - ```display, displays, screen, screens, lcd, lcds``` (no group keywords)
* Door - ```door, hangar, bay, gate```, ```doors, hangars, bays, gates```
* Drill - ```drill```, ```drills```
* Ejector - ```ejector```, ```ejectors```
* Gravity Generator - ```gravitygenerator```, ```gravitygenerators```
* Gravity Sphere Generator - ```gravitysphere```, ```gravityspheres```
* Grid - ```grid, grids``` (no group keywords)
* Grinder - ```grinder```, ```grinders```
* Gun - ```gun, railgun, cannon, autocannon, rocket, missile, launcher```, ```guns, railguns, cannons, autocannons, rockets, missiles, launchers```
* Gyroscope - ```gyro, gyroscope```, ```gyros, gyroscopes```
* HeatVent - ```heatVent```, ```heatVents```
* Hinge - ```hinge```, ```hinges```
* Hydrogen Engine - ```engine```, ```engines```
* Inventory - ```cargo, container, inventory, inventories```, ```cargos, containers```
* Jump Drive - ```jump, jumpdrive```, ```jumps, jumpdrives```
* Laser Antenna - ```laser, laserantenna```, ```lasers, laserantennas```
* Light - ```light, spotlight```, ```lights, spotlights```
* Landing Gear - ```gear, magnet```, ```gears, magnets``` (also Magnets)
* Merge Block - ```merge``` (no group keywords)
* O2/H2 Generator - ```generator```, ```generators```
* Ore Detector - ```detector```, ```detectors```
* Parachute - ```chute, parachute```, ```chutes, parachutes```
* Programmable Block - ```program, programmable```, ```programs```
* Piston - ```piston```, ```pistons```
* Projector - ```projector```, ```projectors``` 
* Reactor - ```reactor```, ```reactors```
* Remote Control - ```remote, drone, robot```, ```remotes, drones, robots```
* Refinery - ```refinery```, ```refineries```
* Rotor - ```rotor```, ```rotors```
* Searchlight - ```searchlight```, ```searchlights```
* Sensor - ```sensor```, ```sensors```
* Solar Panel - ```solar```, ```solars```
* Sorter - ```sorter```, ```sorters```
* Sound Block - ```speaker, alarm, siren```, ```speakers, alarms, sirens```
* Tank (Oxygen or Hydrogen) - ```tank```, ```tanks```
* Terminal Block - ```terminal```, ```terminals```
* Timer Block - ```timer```, ```timers```
* Thruster - ```thruster```, ```thrusters```
* Turret - ```turret```, ```turrets```
* TurretController - ```turretcontroller```, ```turretcontrollers```
* Warhead - ```warhead, bomb```, ```warheads, bombs```
* Welder - ```welder```, ```welders```
* Wheel Suspension - ```wheel```, ```wheels, suspension```
* Wind Turbine - ```turbine```, ```turbines```

## Properties (Inverse in Parentheses, if present)
* Able - ```able, can, ready```, (```unable, cannot```)
* Actions - ```actions```
* Altitude - ```altitude, altitudes, elevation, elevations```
* Angle - ```angle, angles, azimuth, azimuths```
* Artificial - ```artificial, fake```
* Auto - ```auto, autopilot, refill, drain, draining, cooperate, cooperating```
* Background - ```background```
* Color - ```color, foreground```
* Complete - 
  * ```done, complete, finished, built, finish, pressurized, depressurized```
  * (```clear, wipe, erase```)
* Connected -
  * ```connect, connected, attach, attached, dock, docked, docking```
  * (```disconnect, disconnected, detach, detached, undock, undocked```)
* Countdown - ```countdown, countdowns```
* Direction - ```direction, directions```
* Enable - ```enable, enabled, arm, armed```, (```disable, disabled, disarm, disarmed```)
* Falloff - ```falloff```
* Font - ```font```
* Input - ```input, inputs, pilot, pilots, user, users```
* Interval - ```interval```
* Invert - ```invert, inverted, inverting```
* Level - ```height, heights, length, lengths, level, levels, size, sizes, weight, mass```
* Locked - 
  * ```lock, locked, freeze, frozen, brake, braking, handbrake, permanent, static```
  * (```unlock, unlocked, unfreeze```)
* Media - ```sound, music, song, track, image, play, playing, unsilence``` (```silent, silence, quiet```)
* Media List - ```sounds, songs, images, tracks```
* Name - ```name, names, label, labels```
* Natural - ```natural, planet```
* Offset (also Padding)- ```offset, padding```
* Open - ```open, opened``` ```(close, closed, shut)```
* Override - ```override, overrides, overridden, dampener, dampeners```
* Position (also Alignment) - ```position, positions, location, locations, alignment, alignments```
* Power - ```power, powered```
* Properties - ```properties, attributes```
* Range (also Limit) - ```range, ranges, distance, distances, limit, limits, delay, delays, capacity, capacities```
* Radius - ```radius, radii```
* Ratio - ```ratio, ratios, percent, percents, percentage, percentages, progress, progresses```
* Roll Input - ```roll, rolls, rollInput, rollInputs, rotation, rotations```
* Run - ```run, running, execute, executing, script```
* Show - ```show, showing```, (```hide, hiding```)
* Steering - ```steer, steering```
* Strength - ```strength, strengths, force, forces, torque, torques, gravity, gravities```
* Supply - 
  * ```pressure, pressurize, pressurizing, supply, supplying, generate, generating, discharge, discharging, broadcast, broadcasting, assemble, assembling```
  * (```stockpile, stockpiling, depressurize, depressurizing, gather, gathering, intake, recharge, recharging, consume, consuming, collect, collecting, disassemble, disassembling```)
* Target (also Waypoint) - ```target, targeting, destination, waypoint, coords, coordinates```
* Target Velocity - ```targetvelocity```
* Text - ```text, texts, message, messages, argument, arguments```
* Types - ```types, blueprints```
* Trigger - ```trigger, triggered, detect, detected, trip, tripped, deploy, deployed, shoot, shooting, shot, fire, firing, detonate```
* Use - 
  * ```use, used, occupy, occupied, control, controlled```
  * (```unused, unoccupied, vacant, available```)
* Velocity - ```velocity, velocities, speed, speeds, rate, rates, pace, paces```
* Volume (also Output & Intensity) - ```volume, volumes, output, outputs, intensity, intensities```
* Waypoints - ```waypoints, destinations```

### Value Properties
These properties require a Variable value as part of the property
* Amount - ```amount, amounts```
* Action - ```action```
* Property - ```property, attribute```
* Create - ```create, creating, produce, producing, build, building, make, making```
* Destroy -```destroy, destroying, recycle, recycling```

## Commands 
* Iteration - ```times, iterations```
* Wait - ```wait, hold``` 
* Tick - ```tick, ticks``` - wait for ticks instead of seconds
* Call Function - ```call, gosub```
* Goto Function - ```goto```
* Send - ```send```
* Listen - ```listen, channel, register, subscribe```
* Forget - ```forget, dismiss, ignore, deregister, unsubscribe```
* Print - ```print, log, echo, write```
* Queue - ```queue, schedule```
* Async - ```async, parallel```
* Transfer - ```transfer, give```(Source -> Destination)
* Transfer - ```take``` (Destination -> Source)
* For Each - ```each, every```

### Control Commands
* Restart - ```restart, reset, reboot```
* Repeat - ```repeat, loop, rerun, replay```
* Exit - ```exit```
* Pause - ```pause```
* Break - ```break```
* Continue - ```continue```
* Return - ```return```

### Action Words
* Action - ```move, go, tell, turn, rotate, set, apply``` (also can be used for variable assignment)
* Action Upwards - ```raise, extend```
* Action Downwards - ```retract```
* Reverse - ```reverse, reversed```
* Increment -  ```increase, increment, by, ++, +=``` (also used for incrementing variables)
* Decrement - ```decrease, decrement, reduce, --, -=``` (also used for decrementing variables)

### Directions
* Up - ```up, upward, upwards, upper```
* Down - ```down, downward, downwards, lower```
* Left - ```left, lefthand```
* Right - ```right, righthand```
* Forward - ```forward, forwards, front```
* Backward - ```backward, backwards, back```
* Clockwise - ```clockwise, clock```
* Counter Clockwise - ```counter, counterclock, counterclockwise```

## Variables
* Assign - ```set, assign, allocate, designate``` (Also used for Actions)
* Bind - ```bind, tie, link``` - Bind 
* Global - ```global``` - Global Variable
* Increment - ```increase, increment, add, by, ++, +=``` (Also used to increment block properties)
* Decrement - ```decrease, decrement, reduce, subtract, --, -=``` (Also used to decrement block properties)
* Keyed Variable - ```->``` (Used in Collections)

## Conditions
* Condition - ```if, unless, while, until, when```
* Otherwise - ```else, otherwise```

### Aggregate Conditions
* Aggregation Conditions - ```any, all, none```

## Operations
* Parentheses - ```(,)```
* And - ```and, &, &&, but, yet```
* Or - ```or, |, ||```
* Not - ```not, !, stop```
* Abs - ```abs, absolute```
* Square Root - ```sqrt```
* Sin - ```sin```
* Cos - ```cos, cosine```
* Tan - ```tan, tangent```
* Arcsin - ```arcsin, asin```
* Arccos - ```arccos, acos```
* Arctan - ```arctan, atan```
* Sign - ```sign, quantize```
* Random - ```random, rand```
* Round - ```round, rnd, rounded```
* Shuffle - ```shuffle, shuffled```
* Sort - ```sort, sorted```
* Multiply - ```multiply, *```
* Divide - ```divide, /```
* Modulus - ```mod, %```
* Exponent - ```pow, ^, xor``` (also used for Xor and Angle Between Operations)
* Join - ```join, joined```
* Natural Logarithm - ```ln``` 
* Add - ```+, plus```
* Split - ```split, separate, separated```
* Subtract - ```-, minus```
* Dot Product - ```dot, .```
* Ternary - ```?, :```
* Cast - ```as, cast```
* Cast Types - ```bool, number, string, vector, color, list```

### Comparisons
* Less Than - ```less, <, below```
* Less Than Or Equal - ```<=```
* Equal - ```is, are, equal, equals, =, ==```
* Greater Than Or Equal - ```>=```
* Greater Than - ```greater, >, above, more```

## Aggregations
* Average - ```average, avg```
* Count - ```count, number```
* List - ```list, collection```
* Maximum - ```max, maximum```
* Minimum - ```min, minimum```
* Sum - ```sum, total```

## Collections
* List/Index Indicator - ```[]```
* List Item Separator - ```,```
* Keys - ```keys, indexes```
* Values - ```values```
* Keyed Variable - ```->```

### Constants
* Euler's Number - ```e```
* Pi - ```pi```
* Empty Collection ```empty```
* True - ```on, begin, true, start, started, resume, resumed```
* False - ```off, terminate, cancel, end, false, stopped, halt, halted```

## Strings
* Ambiguous String - ```"``` (can also indicate a named selector)
* Explicit String - ```'``` (can wrap ```""```)
* Explicit String - ``` ` ``` (can wrap ```''```)

## Colors
* Common Colors - ```red, blue, green, orange, yellow, white, black```
* Hex Color indicator - ```#``` (if not first character on the line)