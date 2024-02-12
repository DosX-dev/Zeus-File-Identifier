' Source code at GitHub: https://github.com/DosX-dev/Zeus-File-Identifier
' Coded by DosX

Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

Namespace ZeusFileIdentifier
    Class FileScanner
        Private database = String.Empty

        Sub New(databasePath)
            database = databasePath
        End Sub

        Public Function CheckFile(filePath As String)
            If File.Exists(filePath) Then
                If New FileInfo(filePath).Length = 0 Then
                    Return New Signature("Empty file", "-", "", "")
                Else
                    If IsFileBinary(filePath) Then
                        Return IdentifyFileType(filePath)
                    Else
                        Return New Signature("Simple text file", "Text", "", "")
                    End If
                End If
            Else
                Throw New Exception("File not found.")
            End If
        End Function

        Private Function IdentifyFileType(filePath As String) As Signature
            Dim dataBaseLine As List(Of Signature) = LoadSignatures(database)

            Dim fileBytes(500) As Byte
            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                fs.Read(fileBytes, 0, 501)
            End Using

            Dim fileStartsWith As String = BitConverter.ToString(fileBytes).Replace("-", "").ToLower()

            For Each signature In dataBaseLine
                If CompareBytesStartFromMask(signature.HexSignature, fileStartsWith) Then
                    Return signature
                End If
            Next

            Return New Signature("Unknown", "Unknown", "", "")
        End Function

        Private Function LoadSignatures(dbFilePath As String) As List(Of Signature)
            Dim signatures As New List(Of Signature)
            If File.Exists(dbFilePath) Then
                Dim lines As String() = File.ReadAllLines(dbFilePath)
                For Each line As String In lines
                    Dim parts As String() = line.Split("|"c)
                    If parts.Length = 4 AndAlso Not line.StartsWith("#") Then
                        Dim signature As New Signature(parts(0), parts(1), parts(2), parts(3))
                        signatures.Add(signature)
                    End If
                Next
            End If

            Return signatures
        End Function

        Private Function IsFileBinary(filePath As String) As Long
            Dim bufferSize As Long = 1024
            Dim buffer(bufferSize - 1) As Byte

            Using fileStream As FileStream = New FileStream(filePath, FileMode.Open, FileAccess.Read)
                Dim bytesRead As Long = fileStream.Read(buffer, 0, bufferSize)

                For i As Long = 0 To bytesRead - 1
                    If buffer(i) < 32 AndAlso buffer(i) <> 9 AndAlso buffer(i) <> 10 AndAlso buffer(i) <> 13 Then
                        Return True
                    End If
                Next
            End Using

            Return False
        End Function

        Private Function CompareBytesStartFromMask(stringMask As String, bytesSample As String) As Boolean
            Dim regex As New Regex("'(.*?)'"),
            matches As MatchCollection = regex.Matches(stringMask)

            For Each match As Match In matches
                Dim dynamicBytes As String = match.Groups(1).Value,
                dynamicBytesHex As String = BitConverter.ToString(Encoding.ASCII.GetBytes(dynamicBytes)).Replace("-", "")

                stringMask = stringMask.Replace("'" & dynamicBytes & "'", dynamicBytesHex)
            Next

            stringMask = stringMask.ToLower()

            Dim maskPattern As String = Regex.Replace(stringMask, "\?\?", ".."),
            regexPattern As New Regex("^" & maskPattern)

            Return regexPattern.IsMatch(bytesSample)
        End Function

        Public Function CalculateEntropy(filePath As String) As Double
            Dim byteFrequency As New Dictionary(Of Byte, Long)()
            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                Dim bufferSize As Integer = 4096,
                    buffer(bufferSize - 1) As Byte,
                    bytesRead As Integer
                Do
                    bytesRead = fs.Read(buffer, 0, bufferSize)

                    For i As Integer = 0 To bytesRead - 1
                        Dim b As Byte = buffer(i)

                        If byteFrequency.ContainsKey(b) Then
                            byteFrequency(b) += 1
                        Else
                            byteFrequency(b) = 1
                        End If
                    Next

                Loop While bytesRead > 0
            End Using

            Dim totalBytes As Long = New FileInfo(filePath).Length
            Dim entropy As Double = 0.0

            For Each count As Long In byteFrequency.Values
                Dim probability As Double = count / totalBytes
                entropy -= probability * Math.Log(probability, 2)
            Next

            Return entropy
        End Function

        Function GetFileHash(filePath As String, hashAlgorithm As HashAlgorithm) As String
            Try
                Using fileStream As FileStream = File.OpenRead(filePath)
                    Dim hashBytes() As Byte = hashAlgorithm.ComputeHash(fileStream)
                    Return BitConverter.ToString(hashBytes).Replace("-", "").ToLower()
                End Using
            Catch ex As Exception
                Console.WriteLine($"Ошибка при вычислении хеша: {ex.Message}")
                Return String.Empty
            End Try
        End Function

        Function GetFileSize(filePath) As Long
            Return New FileInfo(filePath).Length
        End Function
    End Class

    Class Signature
        Public Property FileTypeName As String
        Public Property FormatType As String
        Public Property FileTypeDescription As String
        Public Property HexSignature As String

        Public Sub New(FormatType As String, signatureType As String, fileTypeDescription As String, hexSignature As String)
            Me.FileTypeName = FormatType
            Me.FormatType = signatureType
            Me.FileTypeDescription = fileTypeDescription
            Me.HexSignature = hexSignature
        End Sub
    End Class

End Namespace