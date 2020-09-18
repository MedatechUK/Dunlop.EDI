Imports System.IO
Imports MedatechUK.CLI
Imports MedatechUK.oData

Module Module1

    Public args As New clArg

    Sub Main(arg() As String)

        With args

            Try
                If .Keys.Contains("?") Then
                    .syntax()
                    .wait()
                    End

                End If

#Region "Seeburger.invoice"

                Dim invoic As seeburger.invoice = .Deserial(
                    New FileInfo(arg(0)),
                    GetType(seeburger.invoice)
                )

                Using l As New Loading("INV", AddressOf hLog)
                    For Each iv As seeburger.invoiceIV In invoic.iv
                        With l
                            With .AddRow(1)
                                .TEXT1 = iv.Vendor
                                .TEXT2 = iv.Doc
                                .TEXT3 = iv.PO
                                .TEXT4 = iv.Ref
                                Select Case iv.Type
                                    Case seeburger.transtype.Credit
                                        .TEXT5 = "C"
                                    Case seeburger.transtype.Debit
                                        .TEXT5 = "D"

                                End Select

                                .REAL1 = iv.Net
                                .REAL2 = iv.Vat
                                .REAL3 = iv.Total

                            End With

                            For Each i As seeburger.invoiceIVItem In iv.item
                                With .AddRow(2)
                                    .TEXT4 = i.Part
                                    .INT1 = i.Qty
                                    .REAL4 = i.Each
                                    .REAL5 = i.Line

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
