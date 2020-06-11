Imports System.IO
Imports MedatechUK.CLI
Imports MedatechUK.oData

Module Module1

    Public args As New clArg

    Sub Main(arg() As String)

        With args

            Try

#Region "Seeburger.Orders"

                Dim orders As seeburger.Orders = .Deserial(
                    New FileInfo(arg(0)),
                    GetType(seeburger.Orders)
                )

                Using l As New Loading("ORD", AddressOf hLog)
                    For Each order As seeburger.OrdersOrder In orders.Order
                        With l
                            With .AddRow(1)
                                .TEXT1 = order.Cust

                            End With

                            For Each i As seeburger.OrdersOrderItem In order.item
                                With .AddRow(2)
                                    .TEXT2 = i.PO
                                    .TEXT3 = i.Part
                                    .TEXT4 = i.Site
                                    .INT1 = i.Qty
                                    .INT2 = i.DateInt

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
