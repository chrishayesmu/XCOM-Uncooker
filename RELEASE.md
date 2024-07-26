This file documents how to create a new release. It is meant for developers of XCOM Uncooker, not users.

# Steps

1. In Visual Studio, build the XCOM-Uncooker solution in the Release configuration.
2. Create a folder called `xcom-uncooker` and copy the contents of `XCOM-Uncooker\bin\Release\net8.0` into it.
3. In Inno Setup Compiler, open `InnoSetup\create-installer.iss` and click Build > Compile along the top.
4. Move `InnoSetup\Output\xcom-uncooker-udk-setup.exe` so it is next to (not inside) the `xcom-uncooker` folder from before.
5. Create a folder called `replacement-packages` next to the `xcom-uncooker` folder.
6. Inside `replacement-packages`, place copies of any UPK files which have been manually modified. Retain the same folder structure that they should have within the UDK, starting with `XComGame` as the top-level folder.
    * Eventually this may be part of the Inno Setup installer instead.
7. Create a git tag at the current commit called `vX.Y.Z`, with the version number of the release. Push the tag to `origin/master`.
8. Create a zip archive containing the folders `xcom-uncooker` and `replacement-packages`, as well as the file `xcom-uncooker-udk-setup.exe`.
9. Upload the zip file to the Github releases as a new release, using the new tag, with appropriate update notes.