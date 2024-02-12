# 🔮 Zeus File Identifier (ZFI)
This program is designed to determine the file type using signatures according to certain rules. Can be used by antiviruses.

## 📑 About
This console program takes an argument `--file` which specifies the path to the file that needs to be examined. The output will contain the resulting format, description and some technical data (for example, the presence of compressed entropy data)

## 💼 Database rules format
```
{STRING_NAME}|{STRING_FILE_TYPE}|{STRING_DESCRIPTION}|{MASK_SIGNATURE}
```
 * Use ".."/"??" to indicate an unknown byte (like "0000..29..11...00")
 * Use the ' character to wrap an ASCII string (like "000000'String in the file!'000029")

The signature is read only from the beginning of the file.

Example of rule:
```
Lua Bytecode|Script|Lua machine code|1B'Lua'..00
```

## 🐧 Run on Linux
Use the `mono-complete` package to use the program on Linux:
```
mono Zeus_File_Identifier.app --file "application.exe"
```

## ✨ What about...
[Detect It Easy](https://github.com/horsicq/DIE-engine) will soon support Zeus File Identifier formats
