# PriBase.dll

## Purpose

A library of Priority form definitions inheriting the oData functions from [oDataCli.dll](https://github.com/MedatechUK/MedatechEDI/tree/master/PriorityForms).

These classes are automatically generated from the API console, and define the specific columns and subforms in the customers installation of Priority.

## oData General Load

Install the [latest shell files](https://github.com/MedatechUK/MedatechEDI/releases).

These contain tables and relevant forms.
- ODATATYPE		: The list of available Priority Loading Interfaces with their TYPENAME (CHAR,3,'Type').
- ODATATRANS	: The transaction, speciftying the ODATATYPE to load, and the system.guid BUBBLEID.
- ODATALOAD		: Contains data linked to the ODATATRANS, including the RECORDTYPE to be used in the interface.
- ODATALINK		: The linked table that is used to pass ODATALINK data to the ODATATYPE interface.
- ODATAERR		: Error from ERRMSGS are recorded against the ODATATRANS when running the ODATATYPE interface.

The client writes to ODATATRANS using the [ODAT_TRANS schema](https://github.com/MedatechUK/MedatechEDI/blob/master/PriorityForms/Schema/ODAT_TRANS.vb), supplying the ODATATYPE TYPENAME and a unique system.guid for the transaction.
```
Using F As New PriBase.ODAT_TRANS(
    Assembly.Load(
        "PriBase"
    )
)
    With F
        With .AddRow()
            .TYPENAME = "ORD"
            .BUBBLEID = System.Guid.NewGuid.ToString
```

The client then populates the ODATALOAD using the sub-level form [ODAT_LOAD](https://github.com/MedatechUK/MedatechEDI/blob/master/PriorityForms/Schema/ODAT_LOAD.vb). Note that you MUST specify the RECORDTYPE for the row. Fill in the other text, real and integer columns as appropriate.
```
            With .ODAT_LOAD.AddRow
                .RECORDTYPE = "1"
				
                .TEXT1 = String.Format("{0} {1}", e.customer.lastName, e.customer.firstName)                
		...
		.REAL1 = e.total_refunded
		...
		.INT1 = DateDiff(DateInterval.Minute, dot, DateTime.Parse(e.created_at))
		...
```

Once the data has been inserted into the ODATALOAD table, the client MUST tell Priority that it finished sending the transaction, by marking it as COMPLETE. This will trigger the loading interface for this transaction.
```
            Using F As New PriBase.ODAT_TRANS(
                Assembly.Load(
                    "PriBase"
                )
            )
                With F
                    With .AddRow()
                        .TYPENAME = "CST"
                        .BUBBLEID = id
                        .COMPLETE = "Y"

                    End With

                    .Post()
                    ErCheck(F)

                End With

            End Using
```