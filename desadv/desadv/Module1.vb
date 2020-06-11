Imports System.IO
Imports MedatechUK.CLI
Imports MedatechUK.oData

Module Module1

    Public args As New clArg

    Sub Main(arg() As String)

        With args

            Try

#Region "Seeburger.Despatch"

                Dim desadv As seeburger.Despatch = .Deserial(
                    New FileInfo(arg(0)),
                    GetType(seeburger.Despatch)
                )

                Using l As New Loading("DSP", AddressOf hLog)
                    For Each vendor As seeburger.DespatchVendor In desadv.Vendor
                        With l
                            With .AddRow(1)
                                .TEXT1 = vendor.Name
                                .TEXT2 = vendor.PO
                                .TEXT3 = vendor.Doc

                            End With

                            For Each part As seeburger.DespatchVendorPart In vendor.Part
                                With .AddRow(2)
                                    .TEXT4 = part.Name
                                    .REAL1 = part.Price
                                    .INT1 = part.Qty

                                End With
                            Next

                            Dim exep As List(Of Exception) = .Post()
                            If exep.Count > 0 Then
                                For Each e As Exception In exep
                                    Console.WriteLine(e.Message)
                                    args.Log(e.Message)

                                Next
                                Throw New Exception("Errors in loading. Please see log.")

                            End If

                        End With
                    Next

                End Using

#End Region

            Catch ex As Exception
                Console.WriteLine(ex.Message)
                .Log(ex.Message)

            Finally
                .wait()

            End Try

        End With

    End Sub

    Private Sub hLog(sender As Object, e As LogArgs)
        Console.WriteLine(e.Message)
        args.Log(e.Message)

    End Sub

End Module
