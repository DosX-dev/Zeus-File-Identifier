' Source code at GitHub: https://github.com/DosX-dev/Zeus-File-Identifier
' Coded by DosX

Imports System.Security.Cryptography

Module Main
    Sub Main(args As String())
        Console.WriteLine("
    ███████ ███████ ██    ██ ███████
       ███  ██      ██    ██ ██     
      ███   █████   ██    ██ ███████
     ███    ██      ██    ██      ██
    ███████ ███████  ██████  ███████
      FILE   IDENTIFIER,   (С) DosX
")

        Try
            Dim filePath As String = GetFilePathFromArgs(args)

            If String.IsNullOrEmpty(filePath) Then
                Console.Out.WriteLine("Missing --file argument")
                Return
            End If

            Dim fileChecker As New ZeusFileIdentifier.FileScanner("zeus.db"),
                result As ZeusFileIdentifier.Signature = fileChecker.CheckFile(filePath)

            Console.Out.WriteLine("[ ANALYSIS ]")
            Console.Out.WriteLine("File type name: " & result.FileTypeName)

            Dim fileTypeDescription As String = result.FileTypeDescription
            Console.Out.WriteLine("File type description: " & (If(fileTypeDescription.Length = 0, "-", fileTypeDescription)))

            Console.Out.WriteLine("File format: " & result.FormatType)

            Console.Out.WriteLine(vbLf & "[ GENERIC ]")

            Dim fileSize As Long = fileChecker.GetFileSize(filePath)
            Console.Out.WriteLine("File size: " & fileSize & " bytes or ~" & (fileSize / 1024) & " kilobytes or ~" & Math.Round(fileSize / 1024 / 1024 / 1024, 4) & " gigabytes")

            Dim fileEntropy As Double = fileChecker.CalculateEntropy(filePath)
            Console.Out.WriteLine("File entropy: " & Math.Round(fileEntropy, 2) & " out of 8 " & If(fileEntropy > 7.0, "[high; like compressed]", "[normal]"))

            Console.Out.WriteLine(vbLf & "[ HASHES ]")

            Dim md5Hash As String = fileChecker.GetFileHash(filePath, New MD5CryptoServiceProvider())
            Console.Out.WriteLine($"MD5 hash: {md5Hash}")

            Dim sha1Hash As String = fileChecker.GetFileHash(filePath, New SHA1CryptoServiceProvider())
            Console.Out.WriteLine($"SHA-1 hash: {sha1Hash}")

            Dim sha256Hash As String = fileChecker.GetFileHash(filePath, New SHA256CryptoServiceProvider())
            Console.Out.WriteLine($"SHA-256 hash: {sha256Hash}")
        Catch ex As Exception
            Console.Out.WriteLine(ex.Message)
        End Try
        Console.WriteLine()
    End Sub

    Function GetFilePathFromArgs(args As String()) As String
        For i As Integer = 0 To args.Length - 2
            If args(i).ToLower() = "--file" Then
                Return args(i + 1)
            End If
        Next
        Return String.Empty
    End Function
End Module