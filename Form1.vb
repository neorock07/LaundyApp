Imports System.Data.Odbc
Imports System.Drawing.Color
Imports iTextSharp.text.pdf
Imports iTextSharp.text
Imports System.IO

Public Class Form1
    'inisiasi variable
    Dim Conn As OdbcConnection
    Dim Cmd As OdbcCommand
    Dim Ds As DataSet
    Dim Da As OdbcDataAdapter
    Dim Ds_1 As DataSet
    Dim Da_1 As OdbcDataAdapter
    Dim Rd As OdbcDataReader
    Dim MyDB As String
    Dim angka As Integer = 0
    Dim msuk As Integer = 0
    Dim sls As Integer = 0
    Dim ttl_bayar As Integer = 0
    Dim ambil As Integer = 0
    Dim cnl As Integer = 0
    Dim fix_angka_1 As Integer = 0
    Dim member As Integer = 0
    Dim i As Integer = 0

    Dim cmdM As OdbcCommand
    Dim cmdS As OdbcCommand
    Dim cmdnama As OdbcCommand
    Dim cmdMember As OdbcCommand
    Dim cmdambil As OdbcCommand
    Dim cmdcnl As OdbcCommand
    Dim cmdlunas As OdbcCommand
    Dim cmdAll As OdbcCommand
    Dim cmdpotang As OdbcCommand
    Dim cmdkas As OdbcCommand
    Dim blm As String = "Belum Lunas"
    'sql command yang dibutuhkan
    Dim exec As String = "SELECT COUNT(*) FROM tb_terima WHERE STATUS_BRG = 'Baru'"
    Dim cmlunas As String = "SELECT COUNT(transaksi) FROM tb_terima WHERE transaksi = 'Lunas'"
    Dim htung_member As String = "SELECT NoAntrian FROM tb_terima ORDER BY NoAntrian DESC LIMIT 8"
    Dim htung_kas As String = "SELECT SUM(JML_UANG) FROM TB_BAYAR"
    Dim cmpotang As String = "SELECT COUNT(transaksi) FROM tb_terima WHERE transaksi = 'Belum Lunas'"
    Dim cmSls As String = "SELECT COUNT(*) FROM tb_terima WHERE STATUS_BRG = 'Selesai'"
    Dim cmcnl As String = "SELECT COUNT(*) FROM tb_terima WHERE STATUS_BRG = 'Batal'"
    Dim cmAmbil As String = "SELECT COUNT(*) FROM tb_terima WHERE STATUS_BRG = 'Diambil'"
    Dim all_data As String = "SELECT * FROM tb_terima"


    'function untuk menampilkan datagridview database
    Sub tampil()
        Da = New OdbcDataAdapter("select * From tb_terima", Conn)
        Ds = New DataSet
        Da.Fill(Ds, "tb_terima")
        DataGridView1.DataSource = Ds.Tables("tb_terima")
        DataGridView2.DataSource = Ds.Tables("tb_terima")
        DataGridView3.DataSource = Ds.Tables("tb_terima")
    End Sub
    'function untuk koneksi ke database lewat odbc
    Sub koneksi()
        MyDB = "Driver={MySql ODBC 3.51 Driver};Database=db_laundry;Server=localhost;uid=root"
        Try
            Conn = New OdbcConnection(MyDB)
            If Conn.State = ConnectionState.Closed Then
                Conn.Open()
            End If
        Catch ex As Exception
            MessageBox.Show("Koneksi gagal", ex.Message)
        End Try
    End Sub
    'menghitung nomer antrian
    Private Function Indek() As Integer
        Dim indeks As String = "SELECT NoAntrian FROM tb_terima ORDER BY NoAntrian DESC LIMIT 8;"
        Dim kueri_indek As String = "SELECT NoAntrian FROM tb_terima ORDER BY NoAntrian DESC LIMIT 1"
        Dim Cmd1 As OdbcCommand = New OdbcCommand(indeks, Conn)
        Dim nomer As Integer = Cmd1.ExecuteScalar()
        Return nomer
    End Function
    'menampilkan chart
    Private Sub Diagram_show()
        'chart
        Me.Chart1.Series("Lunas").Points.AddY(cmdlunas.ExecuteScalar())
        Me.Chart1.Series("Potang").Points.AddY(cmdpotang.ExecuteScalar())
        Me.Chart1.Series("Baru").Points.AddY(angka)
        Me.Chart1.Series("Selesai").Points.AddY(sls)
        Me.Chart1.Series("Diambil").Points.AddY(ambil)
        Me.Chart1.Series("Batal").Points.AddY(cnl)
    End Sub
    'menambahkan nomer dan nama pelanggan ke comboBox
    Private Sub Add_Pelanggan()
        cmdMember = New OdbcCommand(htung_member, Conn)
        Dim i As Integer = 1
        member = cmdMember.ExecuteScalar()
        Dim arr_member(member) As String
        Dim arr_member2(member) As String
        ComboBox3.Items.Clear()
        ComboBox5.Items.Clear()
        'menambahkan hanya yg belum lunas
        For i = 0 To member
            Dim nama_arr As String = "SELECT nama FROM tb_terima " & " WHERE TRANSAKSI = 'Belum Lunas' AND NoAntrian = " & i
            Dim no_arr As String = "SELECT NoAntrian FROM tb_terima " & " WHERE TRANSAKSI = 'Belum Lunas' AND NoAntrian = " & i
            cmdnama = New OdbcCommand(nama_arr, Conn)
            Dim cmdno As OdbcCommand = New OdbcCommand(no_arr, Conn)
            arr_member(i) = cmdno.ExecuteScalar() & "-" & cmdnama.ExecuteScalar()
            If arr_member(i) = "-" Then
                Continue For
            End If
            ComboBox5.Items.Add(arr_member(i))
        Next
        For i = 0 To member
            Dim nama_arr As String = "SELECT nama FROM tb_terima " & " WHERE NoAntrian = " & i
            Dim no_arr As String = "SELECT NoAntrian FROM tb_terima " & " WHERE NoAntrian = " & i
            cmdnama = New OdbcCommand(nama_arr, Conn)
            Dim cmdno As OdbcCommand = New OdbcCommand(no_arr, Conn)
            arr_member2(i) = cmdno.ExecuteScalar() & "-" & cmdnama.ExecuteScalar()
            If arr_member2(i) = "-" Then
                Continue For
            End If
            ComboBox3.Items.Add(arr_member2(i))
        Next
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Call koneksi()
        Call tampil()


        TextBox6.Enabled = False
        TextBox7.Enabled = False
        TextBox8.Enabled = False
        TextBox9.Enabled = False
        TextBox11.Enabled = False

        'tambah nama_pelanggan ke comboBox
        Add_Pelanggan()

        Dim Da_2 As OdbcDataAdapter
        Da_2 = New OdbcDataAdapter("SELECT * FROM tb_terima", Conn)
        Dim Ds_2 As DataSet
        Ds_2 = New DataSet
        Da_2.Fill(Ds_2, "tb_terima")
        Dim dv As DataView
        dv = New DataView(Ds_2.Tables(0), "transaksi = 'Lunas' ", "transaksi Asc", DataViewRowState.CurrentRows)
        DataGridView4.DataSource = dv

        'Menghitung Data ke Dashboard"
        cmdM = New OdbcCommand(exec, Conn)
        cmdS = New OdbcCommand(cmSls, Conn)
        cmdambil = New OdbcCommand(cmAmbil, Conn)
        cmdcnl = New OdbcCommand(cmcnl, Conn)
        cmdlunas = New OdbcCommand(cmlunas, Conn)
        cmdpotang = New OdbcCommand(cmpotang, Conn)
        cmdkas = New OdbcCommand(htung_kas, Conn)

        sls = cmdS.ExecuteScalar()
        angka = cmdM.ExecuteScalar()
        ambil = cmdambil.ExecuteScalar()
        cnl = cmdcnl.ExecuteScalar()

        Label9.Text = angka
        Label10.Text = sls
        Label13.Text = cnl
        Label7.Text = ambil
        TextBox4.Text = Indek() + 1

        Label33.Text = cmdlunas.ExecuteScalar()
        Label35.Text = cmdpotang.ExecuteScalar()
        Dim masuk As Integer = cmdkas.ExecuteScalar()
        Label38.Text = masuk.ToString("C")

        Diagram_show()

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        If TextBox1.Text = "" Or TextBox2.Text = "" Or ComboBox1.Text = "" Or TextBox4.Text = "" Or ComboBox2.Text = "" Then
            MsgBox("Pastikan semua field terisi")
        Else
            Dim dt As String = DateTimePicker1.Value.ToString("yyyy/mm/dd")
            Dim dt1 As String = DateTimePicker1.Value.ToString("yyyy/MM/dd")
            Dim InputData As String = "Insert into tb_terima (NoAntrian, nama, berat, jenis, tanggal, transaksi, STATUS_BRG) values ('" & TextBox4.Text & "','" & TextBox1.Text & "','" & TextBox2.Text & "','" & ComboBox1.Text & "','" & dt1 & "','" & blm & "','" & ComboBox2.Text & "')"
            Cmd = New OdbcCommand(InputData, Conn)
            Cmd.ExecuteNonQuery()
            MsgBox("Input Data Berhasil")

            Call koneksi()
            Call tampil()

            cmdMember = New OdbcCommand(htung_member, Conn)
            Dim i As Integer = 1
            member = cmdMember.ExecuteScalar()
            Dim arr_member(member) As String

            'add user 
            Add_Pelanggan()

            'Menghitung Data ke Dashboard"
            cmdM = New OdbcCommand(exec, Conn)
            cmdS = New OdbcCommand(cmSls, Conn)
            cmdambil = New OdbcCommand(cmAmbil, Conn)
            cmdcnl = New OdbcCommand(cmcnl, Conn)
            cmdlunas = New OdbcCommand(cmlunas, Conn)
            cmdpotang = New OdbcCommand(cmpotang, Conn)
            cmdkas = New OdbcCommand(htung_kas, Conn)

            sls = cmdS.ExecuteScalar()
            angka = cmdM.ExecuteScalar()
            ambil = cmdambil.ExecuteScalar()
            cnl = cmdcnl.ExecuteScalar()

            Label9.Text = angka
            Label10.Text = sls
            Label13.Text = cnl
            Label7.Text = ambil
            TextBox4.Text = Indek() + 1

            Label33.Text = cmdlunas.ExecuteScalar()
            Label35.Text = cmdpotang.ExecuteScalar()
            Dim masuk As Integer = cmdkas.ExecuteScalar()
            Label38.Text = masuk.ToString("C")

            'chart
            Me.Chart1.Series("Lunas").Points.Clear()
            Me.Chart1.Series("Batal").Points.Clear()
            Me.Chart1.Series("Diambil").Points.Clear()
            Me.Chart1.Series("Selesai").Points.Clear()
            Me.Chart1.Series("Potang").Points.Clear()
            Me.Chart1.Series("Baru").Points.Clear()

            Diagram_show()
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If TextBox4.Text = "" Then
            MsgBox("Pastikan semua field terisi")
        Else
            Dim HapusData As String = "delete from tb_terima where NoAntrian='" & TextBox4.Text & "'"
            Cmd = New OdbcCommand(HapusData, Conn)
            Cmd.ExecuteNonQuery()
            MsgBox("Hapus Data Berhasil")
            Call koneksi()
            Call tampil()

            cmdMember = New OdbcCommand(htung_member, Conn)
            Dim i As Integer = 1
            member = cmdMember.ExecuteScalar()
            Dim arr_member(member) As String
            'add user 
            Add_Pelanggan()


            'Menghitung Data ke Dashboard"
            cmdM = New OdbcCommand(exec, Conn)
            cmdS = New OdbcCommand(cmSls, Conn)
            cmdambil = New OdbcCommand(cmAmbil, Conn)
            cmdcnl = New OdbcCommand(cmcnl, Conn)
            cmdlunas = New OdbcCommand(cmlunas, Conn)
            cmdpotang = New OdbcCommand(cmpotang, Conn)
            cmdkas = New OdbcCommand(htung_kas, Conn)

            sls = cmdS.ExecuteScalar()
            angka = cmdM.ExecuteScalar()
            ambil = cmdambil.ExecuteScalar()
            cnl = cmdcnl.ExecuteScalar()

            Label9.Text = angka
            Label10.Text = sls
            Label13.Text = cnl
            Label7.Text = ambil
            TextBox4.Text = Indek() + 1

            Label33.Text = cmdlunas.ExecuteScalar()
            Label35.Text = cmdpotang.ExecuteScalar()
            Dim masuk As Integer = cmdkas.ExecuteScalar()
            Label38.Text = masuk.ToString("C")
            'chart
            Me.Chart1.Series("Lunas").Points.Clear()
            Me.Chart1.Series("Batal").Points.Clear()
            Me.Chart1.Series("Diambil").Points.Clear()
            Me.Chart1.Series("Selesai").Points.Clear()
            Me.Chart1.Series("Potang").Points.Clear()
            Me.Chart1.Series("Baru").Points.Clear()

            Diagram_show()
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        TextBox1.Text = ""
        TextBox2.Text = ""


    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Label7.Text = DateTimePicker1.Value.Date
    End Sub


    Private Sub Button4_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click

        If ComboBox3.Text = "- no - nama-" Or ComboBox4.Text = "- Paket -" Or ComboBox6.Text = "- Status -" Or TextBox3.Text = "" Then
            MsgBox("Pastikan semua field terisi")
        Else
            Dim dt As String = DateTimePicker1.Value.ToString("yyyy/mm/dd")
            Dim dt2 As String = DateTimePicker2.Value.ToString("yyyy/MM/dd")
            'Retrieve berat sebelum
            Dim number_cus() As Char
            Dim no_cus As String = ComboBox3.Text
            number_cus = no_cus.ToCharArray()
            Dim i As Integer = 0
            Dim no_builder As New System.Text.StringBuilder
            'mengambil no antrian
            While number_cus(i) <> "-"
                no_builder.Append(number_cus(i))
                i += 1

            End While

            Dim fix_angka As Integer = CInt(no_builder.ToString)
            Dim cmBerat As String = "SELECT berat FROM tb_terima WHERE NoAntrian = " & fix_angka
            Dim cmdberat As OdbcCommand = New OdbcCommand(cmBerat, Conn)
            Dim weight As Integer = cmdberat.ExecuteScalar()
            sls = cmdS.ExecuteScalar()
            Dim date1 As String = DateTimePicker2.Value.ToString("yyyy/MM/dd")
            Dim upDate As String = "UPDATE tb_terima SET berat = " & (weight + TextBox3.Text) & ",tanggal = " & "'" & date1 & "'" & ",jenis =" & "'" & ComboBox4.Text & "'" & ",STATUS_BRG = " & "'" & ComboBox6.Text & "'" & "WHERE NoAntrian = " & fix_angka
            Cmd = New OdbcCommand(upDate, Conn)
            Cmd.ExecuteNonQuery()
            MsgBox("Update Data Berhasil")

            Call koneksi()
            Call tampil()
            'Menghitung Data ke Dashboard"
            cmdM = New OdbcCommand(exec, Conn)
            cmdS = New OdbcCommand(cmSls, Conn)
            cmdambil = New OdbcCommand(cmAmbil, Conn)
            cmdcnl = New OdbcCommand(cmcnl, Conn)
            cmdlunas = New OdbcCommand(cmlunas, Conn)
            cmdpotang = New OdbcCommand(cmpotang, Conn)
            cmdkas = New OdbcCommand(htung_kas, Conn)

            sls = cmdS.ExecuteScalar()
            angka = cmdM.ExecuteScalar()
            ambil = cmdambil.ExecuteScalar()
            cnl = cmdcnl.ExecuteScalar()

            Label9.Text = angka
            Label10.Text = sls
            Label13.Text = cnl
            Label7.Text = ambil
            TextBox4.Text = Indek() + 1

            Label33.Text = cmdlunas.ExecuteScalar()
            Label35.Text = cmdpotang.ExecuteScalar()
            Dim masuk As Integer = cmdkas.ExecuteScalar()
            Label38.Text = masuk.ToString("C")

            'chart
            Me.Chart1.Series("Lunas").Points.Clear()
            Me.Chart1.Series("Batal").Points.Clear()
            Me.Chart1.Series("Diambil").Points.Clear()
            Me.Chart1.Series("Selesai").Points.Clear()
            Me.Chart1.Series("Potang").Points.Clear()
            Me.Chart1.Series("Baru").Points.Clear()

            Diagram_show()
        End If


    End Sub

    Private Sub TextBox5_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox5.TextChanged

    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click

        Da_1 = New OdbcDataAdapter("SELECT * FROM tb_terima", Conn)
        Ds_1 = New DataSet
        Da.Fill(Ds_1, "tb_terima")
        Dim dv As DataView
        If TextBox5.Text <> "" Then
            dv = New DataView(Ds_1.Tables(0), "NoAntrian = " & TextBox5.Text, "NoAntrian Asc", DataViewRowState.CurrentRows)
            DataGridView3.DataSource = dv
        ElseIf CheckBox1.Checked = True Then
            dv = New DataView(Ds_1.Tables(0), "STATUS_BRG = 'Baru' ", "STATUS_BRG Asc", DataViewRowState.CurrentRows)
            DataGridView3.DataSource = dv
        ElseIf CheckBox2.Checked = True Then
            dv = New DataView(Ds_1.Tables(0), "STATUS_BRG = 'Selesai' ", "STATUS_BRG Asc", DataViewRowState.CurrentRows)
            DataGridView3.DataSource = dv
        ElseIf CheckBox3.Checked = True Then
            dv = New DataView(Ds_1.Tables(0), "STATUS_BRG = 'Diambil' ", "STATUS_BRG Asc", DataViewRowState.CurrentRows)
            DataGridView3.DataSource = dv
        ElseIf CheckBox4.Checked = True Then
            dv = New DataView(Ds_1.Tables(0), "STATUS_BRG = 'Batal' ", "STATUS_BRG Asc", DataViewRowState.CurrentRows)
            DataGridView3.DataSource = dv

        ElseIf CheckBox1.Checked And DateTimePicker3.Value.ToString <> "" Or DateTimePicker4.Value.ToString <> "" Then
            dv = New DataView(Ds_1.Tables(0), "STATUS_BRG = 'Baru' and tanggal >= '" & DateTimePicker3.Value.ToString("yyyy/MM/dd") & "'" & " and tanggal <= '" & DateTimePicker4.Value.ToString("yyyy/MM/dd") & "'", "tanggal Asc", DataViewRowState.CurrentRows)
            DataGridView3.DataSource = dv
        ElseIf CheckBox2.Checked And DateTimePicker3.Value.ToString <> "" Or DateTimePicker4.Value.ToString <> "" Then
            dv = New DataView(Ds_1.Tables(0), "STATUS_BRG = 'Selesai' and tanggal >= '" & DateTimePicker3.Value.ToString("yyyy/MM/dd") & "'" & " and tanggal <= '" & DateTimePicker4.Value.ToString("yyyy/MM/dd") & "'", "tanggal Asc", DataViewRowState.CurrentRows)
            DataGridView3.DataSource = dv
        ElseIf CheckBox3.Checked And DateTimePicker3.Value.ToString <> "" Or DateTimePicker4.Value.ToString <> "" Then
            dv = New DataView(Ds_1.Tables(0), "STATUS_BRG = 'Diambil' and tanggal >= '" & DateTimePicker3.Value.ToString("yyyy/MM/dd") & "'" & " and tanggal <= '" & DateTimePicker4.Value.ToString("yyyy/MM/dd") & "'", "tanggal Asc", DataViewRowState.CurrentRows)
            DataGridView3.DataSource = dv
        ElseIf CheckBox4.Checked And DateTimePicker3.Value.ToString <> "" Or DateTimePicker4.Value.ToString <> "" Then
            dv = New DataView(Ds_1.Tables(0), "STATUS_BRG = 'Batal' and tanggal >= '" & DateTimePicker3.Value.ToString("yyyy/MM/dd") & "'" & " and tanggal <= '" & DateTimePicker4.Value.ToString("yyyy/MM/dd") & "'", "tanggal Asc", DataViewRowState.CurrentRows)
            DataGridView3.DataSource = dv
        ElseIf CheckBox5.Checked And DateTimePicker3.Value.ToString <> "" Or DateTimePicker4.Value.ToString <> "" Then
            dv = New DataView(Ds_1.Tables(0), "transaksi = 'Lunas'", "transaksi Asc", DataViewRowState.CurrentRows)
            DataGridView3.DataSource = dv
        ElseIf CheckBox6.Checked And DateTimePicker3.Value.ToString <> "" Or DateTimePicker4.Value.ToString <> "" Then
            dv = New DataView(Ds_1.Tables(0), "transaksi = 'Belum Lunas'", "transaksi Asc", DataViewRowState.CurrentRows)
            DataGridView3.DataSource = dv
        End If




    End Sub



    Private Sub ComboBox5_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox5.SelectedIndexChanged
        If ComboBox5.Text = "- no - nama-" Then
            MsgBox("Pastikan semua field terisi")
        Else


            Dim number_cus() As Char
            Dim no_cus As String = ComboBox5.Text
            number_cus = no_cus.ToCharArray()
            Dim i As Integer = 0
            Dim no_builder As New System.Text.StringBuilder
            'mengambil no antrian
            While number_cus(i) <> "-"
                If number_cus(i) = "- " Then
                    Continue While
                End If
                no_builder.Append(number_cus(i))
                i += 1
            End While
            fix_angka_1 = CInt(no_builder.ToString)
            Dim cmdPaket As OdbcCommand
            Dim cmdBerat As OdbcCommand


            Dim cmPaket As String = "SELECT jenis FROM tb_terima WHERE NoAntrian = " & fix_angka_1
            Dim cmBerat As String = "SELECT berat FROM tb_terima WHERE NoAntrian = " & fix_angka_1
            cmdPaket = New OdbcCommand(cmPaket, Conn)
            cmdBerat = New OdbcCommand(cmBerat, Conn)
            TextBox6.Text = cmdPaket.ExecuteScalar()
            TextBox7.Text = cmdBerat.ExecuteScalar()

            If TextBox6.Text = "Paket Gembel (Cuci doang) Rp. 5000/Kg" Then
                TextBox8.Text = "5000"
            ElseIf TextBox6.Text = "Paket Rapi (Cuci, Seterika) Rp. 10000/Kg" Then
                TextBox8.Text = "10000"
            Else
                TextBox8.Text = "15000"
            End If
        End If
        Dim str_curr As String
        str_curr = (TextBox7.Text * TextBox8.Text).ToString("C")
        TextBox9.Text = str_curr


    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Try
            Dim str_kembali As String
            ttl_bayar = TextBox9.Text
            str_kembali = (Val(TextBox10.Text) - TextBox9.Text).ToString("C")
            TextBox11.Text = str_kembali
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try

    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Try
            Dim upDate As String = "UPDATE tb_terima SET transaksi = 'Lunas' WHERE NoAntrian = " & fix_angka_1
            Cmd = New OdbcCommand(upDate, Conn)
            Cmd.ExecuteNonQuery()
            MsgBox("Update Data Berhasil")

            'add user
            Add_Pelanggan()

            'insert data into tb_bayar
            Dim cmd_bayar As String = "INSERT INTO TB_BAYAR VALUES ('" & fix_angka_1 & "','" & ttl_bayar & "')"
            Dim cmdBayar As OdbcCommand = New OdbcCommand(cmd_bayar, Conn)
            cmdBayar.ExecuteNonQuery()

            'Menghitung Data ke Dashboard"
            cmdM = New OdbcCommand(exec, Conn)
            cmdS = New OdbcCommand(cmSls, Conn)
            cmdambil = New OdbcCommand(cmAmbil, Conn)
            cmdcnl = New OdbcCommand(cmcnl, Conn)
            cmdlunas = New OdbcCommand(cmlunas, Conn)
            cmdpotang = New OdbcCommand(cmpotang, Conn)
            cmdkas = New OdbcCommand(htung_kas, Conn)

            sls = cmdS.ExecuteScalar()
            angka = cmdM.ExecuteScalar()
            ambil = cmdambil.ExecuteScalar()
            cnl = cmdcnl.ExecuteScalar()

            Label9.Text = angka
            Label10.Text = sls
            Label13.Text = cnl
            Label7.Text = ambil
            TextBox4.Text = Indek() + 1

            Label33.Text = cmdlunas.ExecuteScalar()
            Label35.Text = cmdpotang.ExecuteScalar()
            Dim masuk As Integer = cmdkas.ExecuteScalar()
            Label38.Text = masuk.ToString("C")

            Dim Da_2 As OdbcDataAdapter
            Da_2 = New OdbcDataAdapter("SELECT * FROM tb_terima", Conn)
            Dim Ds_2 As DataSet
            Ds_2 = New DataSet
            Da_2.Fill(Ds_2, "tb_terima")
            Dim dv As DataView
            dv = New DataView(Ds_2.Tables(0), "transaksi = 'Lunas' ", "transaksi Asc", DataViewRowState.CurrentRows)
            DataGridView4.DataSource = dv

            'chart
            Me.Chart1.Series("Lunas").Points.Clear()
            Me.Chart1.Series("Batal").Points.Clear()
            Me.Chart1.Series("Diambil").Points.Clear()
            Me.Chart1.Series("Selesai").Points.Clear()
            Me.Chart1.Series("Potang").Points.Clear()
            Me.Chart1.Series("Baru").Points.Clear()

            Diagram_show()

        Catch ex As Exception
            MsgBox("Eror! :: " & ex.Message.ToString)
        End Try

    End Sub


    Private Sub TabPage5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        Dim cmPkt As String = "SELECT JENIS FROM TB_TERIMA WHERE NoAntrian = " & fix_angka_1
        Dim cmBrt As String = "SELECT BERAT FROM TB_TERIMA WHERE NoAntrian = " & fix_angka_1
        Dim cmdPaket As OdbcCommand = New OdbcCommand(cmPkt, Conn)
        Dim cmdBrt As OdbcCommand = New OdbcCommand(cmPkt, Conn)
        Dim cmNama As String = "SELECT NAMA FROM TB_TERIMA WHERE NoAntrian = " & fix_angka_1
        Dim cmdNm As OdbcCommand = New OdbcCommand(cmNama, Conn)

        Dim pdfDoc As New Document()
        Dim namaPdf As String = ComboBox5.Text & "_Invoice_LaundryApp"
        Dim pdfWrite As PdfWriter = PdfWriter.GetInstance(pdfDoc, New FileStream("D:\LaundryApp\Tugas Akhir\laundryApp\laundryApp\Doc\" & namaPdf & ".pdf", FileMode.Create))
        Dim folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        Dim img = Path.Combine("D:\LaundryApp\Tugas Akhir\laundryApp\laundryApp", "laundry.PNG")
        pdfDoc.Open()
        Dim img1 = iTextSharp.text.Image.GetInstance(img)
        img1.Alignment = 1
        pdfDoc.Add(img1)
        Dim font = iTextSharp.text.Font.BOLD
        Dim header = New Paragraph("INVOICE LAUNDRY APP")
        header.Alignment = 1
        header.Font.Size = 18
        pdfDoc.Add(header)
        Dim address = New Paragraph("PT NEOINT")
        address.Alignment = 1
        address.Font.Size = 16
        pdfDoc.Add(address)
        Dim address1 = New Paragraph("Jln.Gatotkaca, Tipes, Serengan, Surakarta")
        address1.Alignment = 1
        pdfDoc.Add(address1)
        pdfDoc.Add(New Paragraph(""))
        pdfDoc.Add(New Paragraph(""))
        Dim rnd As New Random()
        Dim rnd_num As Integer = rnd.Next(100, 999)
        Dim ths As Date
        ths = Today
        Dim no = New Paragraph("Invoice no : " & rnd_num)
        pdfDoc.Add(no)
        Dim name = New Paragraph("Kepada : " & ComboBox5.Text)
        pdfDoc.Add(name)
        pdfDoc.Add(New Paragraph(ths))

        pdfDoc.Add(New Paragraph(""))
        pdfDoc.Add(New Paragraph(""))
        pdfDoc.Add(New Paragraph("............................................................................................................................................................"))
        pdfDoc.Add(New Paragraph(""))
        pdfDoc.Add(New Paragraph(""))
        pdfDoc.Add(New Paragraph(""))
        pdfDoc.Add(New Paragraph(""))
        Dim item = New Paragraph("Paket                                                                                                                 Berat")
        item.Font.SetStyle(1)
        item.Font.Size = 13
        pdfDoc.Add(item)
        pdfDoc.Add(New Paragraph(""))
        pdfDoc.Add(New Paragraph(""))


        Dim harga As Integer
        If TextBox6.Text = "Paket Gembel (Cuci doang) Rp. 5000/Kg" Then
            harga = CInt(TextBox7.Text) * 5000
        ElseIf TextBox6.Text = "Paket Rapi (Cuci, Seterika) Rp. 10000/Kg" Then
            harga = CInt(TextBox7.Text) * 10000
        Else
            harga = CInt(TextBox7.Text) * 15000
        End If
        Dim jenis = New Paragraph(TextBox6.Text & "                                                                    " & CInt(TextBox7.Text) & " Kg")
        pdfDoc.Add(jenis)
        pdfDoc.Add(New Paragraph(""))
        pdfDoc.Add(New Paragraph(""))
        pdfDoc.Add(New Paragraph("............................................................................................................................................................"))
        Dim total = New Paragraph("Total                                                                                                                             " & harga.ToString("C"))
        total.Font.SetStyle(1)
        pdfDoc.Add(total)
        Dim thk = New Paragraph("Terima Kasih")
        thk.Alignment = 1
        thk.Font.Size = 18
        pdfDoc.Add(thk)


        pdfDoc.Close()
    End Sub

End Class

