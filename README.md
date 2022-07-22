# TinyCommands

TinyCommands adds a handful of small slash commands, [much as the name would suggest](https://tvtropes.org/pmwiki/pmwiki.php/Main/ExactlywhatItSaysOnTheTin). These commands can be used from the chatbox directly, or from macros, or with other plugins, but the ""intended"" usage is primarily from within macros.

## Installation

This is a Dalamud plugin, and as such requires you to be using [QuickLauncher](https://github.com/goatcorp/FFXIVQuickLauncher) to start the game. If you are not already doing so, simply follow the instructions on that page to install it. Then, once you've launched FFXIV using QuickLauncher, follow the instructions on [my plugin repo page](https://github.com/PrincessRTFM/MyDalamudPlugins) to add TinyCommands to your plugin installer list. Install it from there, and you're done!

## Commands

### Arguments

TinyCommands uses a custom argument parser for its commands, allowing GNU-style boolean flags to be specified in the form `-flags`, where each individual letter is another flag being turned on. All commands support the `-h` flag to display their extended help message, including usage and a description. Other flags depend on the command in question.

### Aliases

TinyCommands also uses a custom command registration framework as an interface between itself and [Dalamud](https://github.com/goatcorp/Dalamud/), which the above-mentioned argument parser is actually a part of. Commands registered with this framework are often given aliases, which are _hidden_ commands (not listed in Dalamud's `/xlhelp` or the plugin installer window) that are the same command as the "main" one, simply offering different names to use.

### Command List

#### `/tinyhelp`

##### Aliases

- `/ptinyhelp`
- `/thelp`
- `/pthelp`
- `/tinycmd`
- `/ptinycmd`
- `/tcmd`
- `/ptcmd`

##### Description

This command provides basic, general help when used alone, or can be given the name of a TinyCommands plugin command to display the extended help message for that command. The `-h` flag that all plugin commands accept is redirected to the same functionality as this command provides, so the two are **exactly** equivalent. You can also use `/tinyhelp -h` (or `/tinyhelp tinyhelp` to get extended help details about this command.

##### Flags

- `-a`: display aliases for the given command(s) along with their help text

#### `/tinycmds`

##### Aliases

- `/ptinycmds`
- `/tcmds`
- `/ptcmds`

##### Description

This command lists all TinyCommands plugin commands, optionally with their aliases and help text. Each command listed is given with an approximate usage guide describing the arguments it expects to be given.

##### Flags

- `-a`: include aliases for each command listed
- `-v`: include the extended (TinyCommands-specific) help description for each command (not recommended, long!)

#### `/ifcmd`

##### Aliases

- `/ifthen`

##### Description

The first _conditional_ chat command implemented, takes a set of conditions to test in the flags and a command to execute _if_ the conditions all pass. Technically, the "command" needn't be an actual _command_, since whatever text you provide AFTER the condition flags is simply treated as if you had entered it in the chatbox yourself. As a side effect of this, macro-only commands _cannot_ be used, but manual-only commands _can_, so you can't use `/wait` but you can use `/macrocancel`.

If you do not provide any text to execute on success, a message will be printed to your local chatlog with the results of the test; if it fails, the _first condition tested_ that failed will be reported, which is not related to the order of the flags you pass. If it succeeds, a message will be printed saying so.

##### Flags

:alert: For all flags, use the UPPERCASE letter to invert the meaning.

- `-t`: you have a _normal_ target
- `-f`: you have a _focus_ target
- `-o`: you have a _mouseover_ "target" (see the `<mo>` chat placeholder)
- `-c`: you are in combat
- `-p`: you have a _normal_ target and it is **a player**
- `-n`: you have a _normal_ target and it is **an NPC**
- `-m`: you have a _normal_ target and it is **a minion**
- `-w`: you are unmounted
- `-s`: you are swimming
- `-d`: you are diving
- `-u`: you are flying
- `-i`: you are in an instance
- `-l`: you are using a fashion accessory
- `-r`: you have a weapon drawn (even as a crafter/gatherer)
- `-a`: you are in an alliance

#### `/ifjob`

##### Aliases

- `/ifclass`
- `/whenjob`
- `/whenclass`
- `/job`
- `/class`

##### Description

The third conditional command implemented, takes as a first argument a comma-separated (**NOT** whitespace-separated) list of class/job **abbreviations**, which your current class/job will be checked against. If your current class/job is found in the list, the match succeeds. When _not_ inverted, the test passes when the match succeeds; otherwise, it passes when the match _fails_ instead. All remaining text after the first argument will be treated the same as with `/ifcmd` and effectively sent from your chatbox.

If you don't provide any text to execute, a message will be printed with your current class/job abbreviation. The colour will differ depending on whether the test was conisdered passed (green) or failed (orange), accounting for the `-n` ("not one of") flag.

##### Flags

- `-n`: invert the test ("**not** one of the given jobs")

#### `/ifgp`

##### Aliases

- `/gp`
- `/whengp`

##### Description

The second conditional command implemented (can you tell the readme was written late?), works similar to both `/ifcmd` and `/ifjob` to test your GP. The particular comparison is specified by flag, and the GP threshold (if applicable) is given as the first argument. All remaining text is then sent from your chatbox.

As with both of the others, a status message reporting the result of the test is printed to your local chatlog ONLY if you don't pass any text to execute.

##### Flags

- `-l`: less than (must pass threshold to test against as FIRST argument following flags)
- `-g`: greater than **or** equal to (must pass threshold like with `-l`)
- `-c`: at capacity (do **not** pass a test threshold)

#### `/ifzone`

##### Aliases

- `/ifmap`
- `/ifmapzone`

##### Description

The fourth conditional command implemented, takes as a first argument a comma-separated (**NOT** whitespace-separated) list of numeric map zone IDs, which your current map zone will be checked against. If your current zone is in the list, the match succeeds. When _not_ inverted, the test passes when the match succeeds; otherwise, it passes when the match _fails_ instead. All remaining text after the first argument will be treated the same as with `/ifcmd` and effectively sent from your chatbox.

In order to find the numeric map zone ID for your current zone, use the `-g` flag. If you don't pass any arguments and the `-g` flag is missing, the command help will be printed.

##### Flags

- `-g`: **g**et the numeric map zone ID for your current zone
- `-n`: invert the test ("**not** in one of the listed zones")

#### `/noop`

##### Aliases

- `/nop`
- `/null`

##### Description

Does literally nothing. There are no effects whatsoever. The sole purpose of this command is to allow using `<wait.(delay)>` when `/wait` is unavailable but you don't want anything to happen. If you don't use Macrology, you'll probably never use this.

##### Flags

None. Alternatively, anything that isn't `-h`, because it'll all be ignored anyway.

#### `/timer`

##### Aliases

- `/ptimer`
- `/delay`
- `/pdelay`

##### Description

The first command ever implemented, to make a counterpart to the builtin `/alarm` command. This command takes a _delay_ and an alarm name, calculates the (local) time that will come after waiting for the given delay, and creates a vanilla FFXIV alarm for that time with the given name. This allows you to set an alarm to go off _after_ a certain period, rather than _at_ a given time.

This command uses a variant custom parser (the original one implemented, actually) that also splits words, so if your alarm name contains spaces then you will need to enclose it in **double quotes** (`"`) to work.

Fun fact: the inspiration for this command was that I wanted to easily set an alarm to go off when my tea was done steeping but didn't feel like calculating the time myself, so I made a way to simply run `/timer 5m tea` and be done with it.

##### Flags

- `-v`: display the `/alarm` command that was generated, then run it
- `-d`: display the generated `/alarm` command, but _don't_ run it

#### `/echoerr`

##### Aliases

- `/echoerror`
- `/error`
- `/eerr`
- `/err`
- `/ee`

##### Description

It works just like the builtin `/echo` command, but it sends messages on the "error" channel instead. The idea is to be used with this plugin's conditional commands so that an emote macro can warn you if you need to have a target but don't, for example. An actual implementation of that would be simple, and might look similar to this:

```
/macroicon Poke emote
/ifcmd -t /poke motion
/ifcmd -t /em prods <t> gently to get their attention
/ifcmd -T /error You must have a target to use this!
```

This example macro would effectively change the message for the `/poke` animation, and also prevent you from using it unless you have a target. It doesn't make much sense to prod the air, after all.

##### Flags

- `-p`: display the error message with the plugin's usual prefix text, instead of as a "bare" message

#### `/playsound`

##### Aliases

- `/playsfx`

##### Description

It can be annoying to keep `/echo`ing the `<se.##>` placeholders to find one you want to use for something, especially with the way it clutters up your chatlog. This won't save you from needing to change the ID each time, but at least now it's a single backspace and there's no output message. Unless something goes wrong, like using an invalid index.

##### Flags

None.

#### `/whereis`

##### Aliases

- `/locate`

##### Description

If you've ever needed to find something - an NPC in a crowded market, an interactible in the world for a quest, etc - and you've known the name but not where it _is_, this command is precisely what you need. It takes a (case-insensitive) partial name filter and looks for anything near you by that name. Whatever it finds, it prints to your chat, with the full name, distance from you, and current coordinates. It even works on players, if you lose your friend in a city. The range is limited for technical reasons (it can only scan things your client is aware of) so it won't work across the whole entire map, but that's not something that can be fixed.

For added convenience, you can change the sorting order from alphabetical to by-distance (nearest first) and even automatically place your map flag at the location of the first result found, in sort order - so you can drop your marker right on the nearest instance of whatever you're looking for.

##### Flags

- `-d`: sort the list by distance from you (nearest first) instead of alphabetical (a-z)
- `-i`: reverse the list (z-a alphabetical, or farthest first distance)
- `-f`: place your map marker at the first result of the sorted list (the big map will not be opened)
- `-a`: show all results, ignoring ghost checks - all game objects exist in the world at all times, even if they're not rendered or interactable
- `-A`: show all _types_ of results, rather than just the ones people usually want (NPCs, players, companions, quest targets) - this may be needed to return aether currents in the list, so if you can't find any, try it out

#### `/clearflag`

##### Aliases

- `/unflag`

##### Description

Remove your map marker with a command. This allows you to make a macro which can be hotkeyed, or to use another plugin, or even to use conditional commands from this plugin, in order to remove your map flag without having to open your map, find the flag, and right click it.

##### Flags

None. Including your map flag, even.

