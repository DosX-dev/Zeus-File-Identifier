# 🔮 Zeus File Identifier (ZFI)
This program is designed to determine the file type using signatures according to certain rules. Can be used by antiviruses.

## About
This console program takes an argument `--file` which specifies the path to the file that needs to be examined. The output will contain the resulting format, description and some technical data (for example, the presence of compressed entropy data)

## Run on Linux
Use the `mono-complete` package to use the program on Linux:
```
mono Zeus_File_Identifier.app --file "application.exe"
```
