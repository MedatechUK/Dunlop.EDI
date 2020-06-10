Imports System.IO
Imports MedatechUK.CLI
Imports MedatechUK.oData

Module Module1

    Public args As New clArg

    Sub Main()

        With args

            Try

#Region "Seeburger.Orders"

                Dim orders As Seeburger.Orders = .Deserial(
                    New FileInfo("M:\Dunlop.EDI\Order\seeburger.order.v1.xml"),
                    GetType(Seeburger.Orders)
                )

                Using l As New Loading("ORD", AddressOf hLog)
                    For Each order As Seeburger.OrdersOrder In orders.Order
                        With l
                            With .AddRow(1)
                                .TEXT1 = order.Cust

                            End With

                            For Each i As Seeburger.OrdersOrderItem In order.item
                                With .AddRow(2)
                                    .TEXT2 = i.PO
                                    .TEXT3 = i.Part
                                    .TEXT4 = i.Site
                                    .INT1 = i.Qty
                                    .INT2 = i.DateInt

                                End With
                            Next

                            '.Post()

                        End With
                    Next

                End Using

#End Region

#Region "Seeburger.Despatch"

                Dim desadv As Seeburger.Despatch = .Deserial(
                    New FileInfo("M:\Dunlop.EDI\desadv\seeburger.desadv.v1.xml"),
                    GetType(Seeburger.Despatch)
                )

                Using l As New Loading("DSP", AddressOf hLog)
                    For Each vendor As Seeburger.DespatchVendor In desadv.Vendor
                        With l
                            With .AddRow(1)
                                .TEXT1 = vendor.Name
                                .TEXT2 = vendor.PO
                                .TEXT3 = vendor.Doc

                            End With

                            For Each part As Seeburger.DespatchVendorPart In vendor.Part
                                With .AddRow(2)
                                    .TEXT4 = part.Name
                                    .REAL1 = part.Price
                                    .INT1 = part.Qty

                                End With
                            Next

                            '.Post()

                        End With
                    Next

                End Using

#End Region

#Region "Seeburger.invoice"

                Dim invoic As Seeburger.invoice = .Deserial(
                    New FileInfo("M:\Dunlop.EDI\invoic\seeburger.invoic.v1.xml"),
                    GetType(Seeburger.invoice)
                )

                Using l As New Loading("INV", AddressOf hLog)
                    For Each iv As Seeburger.invoiceIV In invoic.iv
                        With l
                            With .AddRow(1)
                                .TEXT1 = iv.Vendor
                                .TEXT2 = iv.Doc
                                .TEXT3 = iv.PO
                                .TEXT4 = iv.Ref
                                Select Case iv.Type
                                    Case Seeburger.transtype.Credit
                                        .TEXT5 = "Y"
                                    Case Seeburger.transtype.Debit
                                        .TEXT5 = "N"

                                End Select

                                .REAL1 = iv.Net
                                .REAL2 = iv.Vat
                                .REAL3 = iv.Total


                            End With

                            For Each i As Seeburger.invoiceIVItem In iv.item
                                With .AddRow(2)
                                    .TEXT4 = i.Part
                                    .INT1 = i.Qty
                                    .REAL4 = i.Each
                                    .REAL5 = i.Line

                                End With
                            Next

                            '.Post()

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
