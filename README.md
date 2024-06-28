# XCOM Uncooker

The **XCOM Uncooker** is a project for XCOM: Enemy Within that allows you to (mostly) reverse the game's cooking process, making its assets openable in the Unreal Development Kit.

# First time setup

Follow these steps to uncook and view the game assets. Note that some warnings during the UDK steps are normal, but errors are not; stop if you hit an error and contact me.

0. You must own XCOM: Enemy Within and have it installed. I have only tried this with [the Steam version](https://store.steampowered.com/app/225340/XCOM_Enemy_Within/), and only with the Enemy Within DLC.
1. Create a **clean and unused** installation of the September 2011 UDK release. You can get it [on this GitHub](https://github.com/chrishayesmu/XCOM-Uncooker/releases/tag/udk-installer) or from [Nexus Mods](https://www.nexusmods.com/xcom/mods/485?tab=description). This absolutely **MUST** be a new install which you aren't using for anything else. It will **not** be able to compile scripts, so if you want to create code mods for XCOM, you need two separate installations of the UDK. The installer is going to delete a bunch of stuff from the UDK folder, so please follow this instruction.
2. Download the latest [XCOM Uncooker release](https://github.com/chrishayesmu/XCOM-Uncooker/releases) and unzip it. It should contain a folder called `xcom-uncooker` and a file called `xcom-uncooker-udk-setup.exe`.
3. Run `xcom-uncooker-udk-setup.exe` and follow the installer's directions. This will modify your brand new UDK install so it can handle XCOM's assets.
4. **Warning: this next step will take a while and bog down your computer while it's running.** The uncooking process is resource intensive and usually takes about 20 minutes on my machine.
   1. Once the UDK setup installer is done, run `xcom-uncooker\XCOM-Uncooker.exe`.
   2. When the uncooker asks for XCOM's `CookedPCConsole` folder, make sure to use the one under the `XEW` folder (e.g. `C:\Steam\steamapps\common\XCom-Enemy-Unknown\XEW\XComGame\CookedPCConsole`).
   3. When the uncooker asks for an output path, use `<UDK install path>\UDKGame\Content`. In my install, this is `C:\UDK\UDKGame\Content`.
   4. Let the uncooker run. It may appear to get stuck now and again due to particularly large or complex files.
5. When the uncooker is complete, go to your UDK installation folder (e.g. `C:\UDK`) and run the "Update Asset Database" shortcut. It should ask if you want to compile scripts; say yes, wait for compilation to finish, then run the shortcut again.
   * Updating the asset database lets you see all of the game assets in the editor, without having to load each individual UPK file.
   * This is optional but highly recommended. It may take a minute or two.
6. When the asset database update is complete, you can close its window and click the other shortcut, "Open UDK Editor".

At this point, you should be able to browse the game assets in the UDK!

# Known issues

## Shaders/textures are missing, objects render as all black

## Objects don't have thumbnails in the editor until double clicking or their containing UPK is fully loaded

## Node-based editor (Kismet, AnimTrees) have their nodes in the wrong spots

# Credits

TBD