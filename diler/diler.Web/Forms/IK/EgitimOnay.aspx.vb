Imports System.IO
Imports System.IO.TextWriter
Imports System.Data
Imports System.Math
Imports System.Text
Imports System.Drawing
Imports Microsoft.Win32
Imports System.Data.Sql
Imports System.Data.Odbc
Imports System.Threading
Imports System.Data.OleDb
Imports System.Web.UI.Page
Imports System.Data.SqlClient
Imports System.Drawing.Printing
Imports System.Threading.Thread
Imports System.Data.SqlClient.SqlDataAdapter
Imports FarPoint.Web.Spread
Imports System.Drawing.Image
Imports System.Drawing.Bitmap
Imports System.Web.UI.WebControls.Image
Imports System.Web.UI.WebControls.ImageMap
Imports System.Globalization

Imports Oracle.ManagedDataAccess.Client
Imports Oracle.ManagedDataAccess.Types


Namespace spWebProductTour.SpreadWebTour
    Partial Class _default
        Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region
        Dim instance As FpSpread
        Dim value As Integer
        Dim SQL, SQL2, SQL3, SQL4, SQL5, KULLANICI, IZIN, IKID, Aciklamax, DonenTarih, DonenTersTarih1, IKADSADX
        Dim GIRENADI, GIRENSAD, GIRENUNITE, GIRENBOLUM, IDX, GIRENUNVAN, GIRENALTGRUP, SecilenSheetName, KayitYapan, ToplamSheet, IKDegerlendirilenID, IKDegerlendirilenAdSoyad
        Dim SecilenSheetGrupNo, SuankiSheetNo, YilX, DonemX As Integer
        Public YoneticiDrm, mesaj, SIFRESI

        Dim ToplamSfr, ToplamBir, ToplamIk, ToplamUc, ToplamDort, ToplamBes


        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load

            If Not Page.IsPostBack Then
                IKADSADX = ("IKADSAD")
                GIRIS()
                filtre()
                GIRENKIM()
                GIRENIN_ALTLARINI_GETIR()
                txtOnayTar.Text = DateTime.Now.ToString("dd/MM/yyyy")
            Else
                GIRENKIM()
            End If

            'EgitimGridDoldur()
        End Sub


        Private Sub GIRIS()

            Dim DbConn As New ConnectOracleDilerIK
            KULLANICI = Session("KULLANICI")
            SIFRESI = Session("SIFRESI")
            SQL = ""
            SQL = "SELECT URTTNM.USRTNM.PERFORMANS,URTTNM.USRTNM.IKID,URTTNM.USRTNM.ADSOYAD FROM URTTNM.USRTNM WHERE URTTNM.USRTNM.USERNAME='" & UCase(KULLANICI) & "' AND URTTNM.USRTNM.SIFRE='" & UCase(SIFRESI) & "'"
            DbConn.RaporWhile(SQL)
            While DbConn.MyDataReader.Read
                IZIN = DbConn.MyDataReader.GetString(0)
                IKID = DbConn.MyDataReader.GetString(1)
                IKADSADX = DbConn.MyDataReader.GetString(2)
            End While
            DbConn.Kapat()

            Session("IKID") = IKID
            Session("IKADSAD") = IKADSADX

        End Sub

        Public Sub AspNetMsgFunc(ByVal sMsg As String)
            Try
                Dim sb As New StringBuilder()
                Dim oFormObject As System.Web.UI.Control
                sMsg = sMsg.Replace("'", "\'")
                sMsg = sMsg.Replace(Chr(34), "\" & Chr(34))
                sMsg = sMsg.Replace(vbCrLf, "\n")
                sMsg = "<script language=javascript>alert(""" & sMsg & """)</script>"
                sb = New StringBuilder()
                sb.Append(sMsg)
                For Each oFormObject In Me.Controls
                    If TypeOf oFormObject Is HtmlForm Then
                        Exit For
                    End If
                Next
                '  oFormObject.Controls.AddAt(oFormObject.Controls.Count, New LiteralControl(sb.ToString()))
            Catch ex As Exception
            End Try
        End Sub

        Sub filtre()
            FarPoint.Web.Spread.DefaultSkins.GetAt(13).Apply(fpPersonelList.Sheets(0))
            fpPersonelList.CommandBar.BackColor = Color.Wheat
            Dim hideRowFilter As New FarPoint.Web.Spread.HideRowFilter(fpPersonelList.ActiveSheetView)
            hideRowFilter.ShowFilterIndicator = False '- Hide Spread's built-in column header filter
            hideRowFilter.AddColumn(4)
            fpPersonelList.ActiveSheetView.RowFilter = hideRowFilter 'Apply the filter
            Dim filterItemList As ArrayList = fpPersonelList.ActiveSheetView.GetDropDownFilterItems(4)
            If Not filterItemList Is Nothing Then
                Dim i As Integer
                For i = 0 To filterItemList.Count - 1
                    drpBolumler.Items.Add(filterItemList.Item(i))
                Next
            End If
        End Sub
        Sub Grupla()

            Dim rc As Integer = 20   '000
            Dim cc As Integer = 8   '0
            Dim m As New FarPoint.Web.Spread.Model.DefaultSheetDataModel(rc, cc)
            Dim gm As New FarPoint.Web.Spread.Model.GroupDataModel(m)

            fpPersonelList.Sheets(0).RowHeader.Visible = False
            fpPersonelList.Sheets(0).AllowPage = False
            fpPersonelList.Sheets(0).AllowSort = True

            fpPersonelList.Sheets(0).ColumnHeaderAutoText = FarPoint.Web.Spread.HeaderAutoText.Blank
            fpPersonelList.Sheets(0).Columns(0).Width = 20
            fpPersonelList.Sheets(0).Columns(0).BackColor = Color.LightYellow
            'Title column
            fpPersonelList.Sheets(0).Columns(4).Width = 175
            fpPersonelList.ActiveSheetView.AllowGroup = True
            fpPersonelList.ActiveSheetView.GroupBarVisible = True
            fpPersonelList.ActiveSheetView.AllowColumnMove = True
            'Set first level grouping details
            Dim gi As New FarPoint.Web.Spread.GroupInfo
            gi.BackColor = Color.LightSteelBlue
            gi.ForeColor = Color.Black
            fpPersonelList.ActiveSheetView.GroupInfos.Add(gi)
            'Set second level grouping details
            gi = New FarPoint.Web.Spread.GroupInfo
            gi.ForeColor = Color.Black  'Navy
            fpPersonelList.ActiveSheetView.GroupInfos.Add(gi)
            'add additional levels if desired, like above, here
            fpPersonelList.ShowFocusRectangle = True

        End Sub
        Sub InitSpread()
            fpPersonelList.ScrollBarBaseColor = Color.LightSteelBlue
            With fpPersonelList.Sheets(0)
                .HeaderGrayAreaColor = Color.LightSteelBlue
                Dim ch As New FarPoint.Web.Spread.LabelCellType
                ch.BackgroundImageUrl = "images/greenbk.jpg"
                .ColumnHeader.DefaultStyle.CellType = ch
                .ColumnHeader.DefaultStyle.ForeColor = Color.White
                .ColumnHeader.DefaultStyle.Font.Name = "Verdana"
                .SheetCornerStyle.CellType = ch
            End With
        End Sub

        Private Sub GIRENKIM()
            IKID = Session("IKID")
            'IKID = 1001690
            '' GİREN ŞEF İSE
            SQL = ""
            SQL = "SELECT IK.NUFUS.ID,IK.NUFUS.ADI,IK.NUFUS.SOYADI,IK.UNITESI.UNITE,IK.UNITESI.BOLUM,IK.UNITESI.ALT_GRUP FROM IK.NUFUS INNER JOIN IK.UNITESI ON IK.NUFUS.ID=IK.UNITESI.ID " _
            & " WHERE IK.UNITESI.ID='" & IKID & "'" _
            & " AND IK.NUFUS.DRM=1" _
            & " AND IK.UNITESI.DURUMU='AKTIF'" _
            & " AND IK.UNITESI.ALT_GRUP LIKE '%ŞEFİ%'"
            '& " OR IK.UNITESI.ALT_GRUP LIKE '%HEKİMİ%'" _
            '& " ORDER BY ID "
            Dim DbConn1 As New ConnectOracleDilerIK
            DbConn1.RaporWhile(SQL)
            While DbConn1.MyDataReader.Read
                GIRENUNVAN = "ŞEF"
                KayitYapan = 2
                IDX = DbConn1.MyDataReader.GetValue(0).ToString()
                GIRENADI = DbConn1.MyDataReader.GetValue(1).ToString()
                GIRENSAD = DbConn1.MyDataReader.GetValue(2).ToString()
                GIRENUNITE = DbConn1.MyDataReader.GetValue(3).ToString()
                GIRENBOLUM = DbConn1.MyDataReader.GetValue(4).ToString()
                GIRENALTGRUP = DbConn1.MyDataReader.GetValue(5).ToString()

            End While
            DbConn1.Kapat()

            '' GİREN MÜDÜR İSE
            SQL = ""
            SQL = "SELECT IK.NUFUS.ID,IK.NUFUS.ADI,IK.NUFUS.SOYADI,IK.UNITESI.UNITE,IK.UNITESI.BOLUM,IK.UNITESI.ALT_GRUP FROM IK.NUFUS INNER JOIN IK.UNITESI ON IK.NUFUS.ID=IK.UNITESI.ID " _
            & " WHERE IK.UNITESI.ID='" & IKID & "'" _
            & " AND IK.UNITESI.DURUMU='AKTIF'" _
            & " AND (IK.UNITESI.ALT_GRUP LIKE '%MÜDÜRÜ%' OR IK.UNITESI.ALT_GRUP LIKE '%MÜDÜR Y%')"
            '& " AND IK.UNITESI.ALT_GRUP NOT LIKE '%MÜDÜRÜ%'"
            Dim DbConn2 As New ConnectOracleDilerIK
            DbConn2.RaporWhile(SQL)
            While DbConn2.MyDataReader.Read
                GIRENUNVAN = "MÜDÜR"
                KayitYapan = 3
                IDX = DbConn2.MyDataReader.GetValue(0).ToString()
                GIRENADI = DbConn2.MyDataReader.GetValue(1).ToString()
                GIRENSAD = DbConn2.MyDataReader.GetValue(2).ToString()
                GIRENUNITE = DbConn2.MyDataReader.GetValue(3).ToString()
                GIRENBOLUM = DbConn2.MyDataReader.GetValue(4).ToString()
                GIRENALTGRUP = DbConn2.MyDataReader.GetValue(5).ToString()
            End While
            DbConn2.Kapat()
            '' GİREN MÜHENDİS İSE
            SQL = ""
            SQL = "SELECT IK.NUFUS.ID,IK.NUFUS.ADI,IK.NUFUS.SOYADI,IK.UNITESI.UNITE,IK.UNITESI.BOLUM,IK.UNITESI.ALT_GRUP FROM IK.NUFUS INNER JOIN IK.UNITESI ON IK.NUFUS.ID=IK.UNITESI.ID " _
            & " WHERE IK.UNITESI.ID='" & IKID & "'" _
            & " AND IK.UNITESI.DURUMU='AKTIF'" _
            & " AND IK.UNITESI.ALT_GRUP LIKE '%MÜHENDİSİ%'"
            Dim DbConn3 As New ConnectOracleDilerIK
            DbConn3.RaporWhile(SQL)
            While DbConn3.MyDataReader.Read
                GIRENUNVAN = "MÜHENDİSİ"
                KayitYapan = 1
                IDX = DbConn3.MyDataReader.GetValue(0).ToString()
                GIRENADI = DbConn3.MyDataReader.GetValue(1).ToString()
                GIRENSAD = DbConn3.MyDataReader.GetValue(2).ToString()
                GIRENUNITE = DbConn3.MyDataReader.GetValue(3).ToString()
                GIRENBOLUM = DbConn3.MyDataReader.GetValue(4).ToString()
                GIRENALTGRUP = DbConn3.MyDataReader.GetValue(5).ToString()
            End While
            DbConn3.Kapat()

            SQL = ""
            SQL = "SELECT IK.NUFUS.ID,IK.NUFUS.ADI,IK.NUFUS.SOYADI,IK.UNITESI.UNITE,IK.UNITESI.BOLUM,IK.UNITESI.ALT_GRUP FROM IK.NUFUS INNER JOIN IK.UNITESI ON IK.NUFUS.ID=IK.UNITESI.ID " _
            & " WHERE IK.UNITESI.ID='" & IKID & "'" _
            & " AND IK.UNITESI.DURUMU='AKTIF'" _
            & " AND IK.UNITESI.ALT_GRUP LIKE '%FABRİKA MÜDÜRÜ%'"
            Dim DbConnFM As New ConnectOracleDilerIK
            DbConnFM.RaporWhile(SQL)
            While DbConnFM.MyDataReader.Read
                GIRENUNVAN = "FABRİKA MÜDÜRÜ"
                KayitYapan = 4
                IDX = DbConnFM.MyDataReader.GetValue(0).ToString()
                GIRENADI = DbConnFM.MyDataReader.GetValue(1).ToString()
                GIRENSAD = DbConnFM.MyDataReader.GetValue(2).ToString()
                GIRENUNITE = DbConnFM.MyDataReader.GetValue(3).ToString()
                GIRENBOLUM = DbConnFM.MyDataReader.GetValue(4).ToString()
                GIRENALTGRUP = DbConnFM.MyDataReader.GetValue(5).ToString()
            End While
            DbConnFM.Kapat()

            If GIRENUNVAN = "FABRİKA MÜDÜRÜ" And IKADSADX = "İnsan Kaynakları" Then
                GirenBilgisi.Text = "Programa Giriş Yapan  İnsan Kaynakları Kullanıcısı..."
            Else
                GirenBilgisi.Text = "Programa Giriş Yapan  " & IDX & "  Sicil Numaralı  " & GIRENBOLUM & "  " & GIRENADI & "  " & GIRENSAD & "  " & GIRENALTGRUP
            End If

            If IKID = "7001111" Then
                GIRENUNVAN = "MÜDÜR"
                KayitYapan = 2
                IDX = "7001111"
                GIRENADI = "MURAT"
                GIRENSAD = "DURAK"
                GIRENUNITE = "Diler Liman Müdürlüğü"
                GIRENBOLUM = "Diler Liman Müdürlüğü"
                GIRENALTGRUP = "Diler Liman Müdürlüğü"
            End If

            If IKID = "3698" Then
                GIRENUNVAN = "MÜDÜR"
                KayitYapan = 3
                IDX = "3698"
                GIRENADI = "SİNAN"
                GIRENSAD = "EVCİMEN"
                GIRENUNITE = "Diler Liman Müdürlüğü"
                GIRENBOLUM = "Diler Liman Müdürlüğü"
                GIRENALTGRUP = "Diler Liman Müdürlüğü"
            End If

            If IKID = "1002411" Then ' Osman Öztürk fabrika müdürü gibi herkesi görecek ve onaylayaca
                GIRENUNVAN = "FABRİKA MÜDÜRÜ"
                KayitYapan = 3
                IDX = "1002411"
                GIRENADI = "DERYA"
                GIRENSAD = "BAYKAL GUCLU"
                GIRENUNITE = "Fabrika Müdürlüğü"
                GIRENBOLUM = "FABRİKA MÜDÜRÜ"
                GIRENALTGRUP = "FABRİKA MÜDÜRÜ"
            End If


            Session("GIRENUNITE") = GIRENUNITE
            Session("GIRENBOLUM") = GIRENBOLUM
            Session("GIRENUNVAN") = GIRENUNVAN
        End Sub

        Private Sub GIRENIN_ALTLARINI_GETIR()
            Dim i = 0
            fpPersonelList.Sheets(0).RowCount = 0
            GIRENUNITE = Session("GIRENUNITE")
            GIRENBOLUM = Session("GIRENBOLUM")
            GIRENUNVAN = Session("GIRENUNVAN")
            If GIRENUNVAN = "FABRİKA MÜDÜRÜ" Then
                drpBolumler.Items.Clear()
                'TAŞERON FIRMA
                SQL = ""
                SQL = "SELECT DISTINCT (IK.UNITESI.ALT_GRUP) FROM IK.NUFUS INNER JOIN IK.UNITESI ON IK.NUFUS.ID=IK.UNITESI.ID " _
                & " WHERE IK.UNITESI.ID<>" & IKID _
                & " AND IK.NUFUS.DRM=" & 1 _
                & " AND IK.UNITESI.DURUMU='AKTIF'" _
                & " AND IK.UNITESI.BOLUM<>'TAŞERON FIRMA'" _
                & " ORDER BY IK.UNITESI.ALT_GRUP"
                Dim DbConnFM As New ConnectOracleDilerIK
                DbConnFM.RaporWhile(SQL)
                While DbConnFM.MyDataReader.Read
                    drpBolumler.Items.Add(DbConnFM.MyDataReader.GetValue(0).ToString())
                End While
                DbConnFM.Kapat()
            End If
            '& " AND (IK.UNITESI.ALT_GRUP NOT LIKE '%ŞEF%')" _

            If GIRENUNVAN = "MÜDÜR" Then
                SQL = ""
                SQL = "SELECT IK.NUFUS.ID,IK.NUFUS.ADI,IK.NUFUS.SOYADI,IK.UNITESI.UNITE,IK.UNITESI.BOLUM,IK.UNITESI.ALT_GRUP FROM IK.NUFUS INNER JOIN IK.UNITESI ON IK.NUFUS.ID=IK.UNITESI.ID " _
                & " WHERE IK.UNITESI.UNITE='" & GIRENUNITE & "'" _
                & " AND IK.UNITESI.ID<>" & IKID _
                & " AND IK.UNITESI.DURUMU='AKTIF'" _
                & " AND IK.UNITESI.BOLUM<>'TAŞERON FIRMA'" _
                & " AND IK.NUFUS.DRM=" & 1 _
                & " AND (IK.UNITESI.ALT_GRUP NOT LIKE '%MÜDÜRÜ%')"
                If drpBolumler.Text <> "(All)" And drpBolumler.Text <> "" Then
                    SQL = SQL & " AND IK.UNITESI.ALT_GRUP ='" & drpBolumler.Text & "'"
                End If
                Dim DbConn As New ConnectOracleDilerIK
                DbConn.RaporWhile(SQL)
                While DbConn.MyDataReader.Read
                    fpPersonelList.Sheets(0).RowCount = fpPersonelList.Sheets(0).RowCount + 1
                    fpPersonelList.Sheets(0).Cells(i, 0).Text = DbConn.MyDataReader.GetValue(0).ToString()
                    fpPersonelList.Sheets(0).Cells(i, 1).Text = DbConn.MyDataReader.GetValue(1).ToString() & " " & DbConn.MyDataReader.GetValue(2).ToString()
                    fpPersonelList.Sheets(0).Cells(i, 2).Text = DbConn.MyDataReader.GetValue(3).ToString()
                    fpPersonelList.Sheets(0).Cells(i, 3).Text = DbConn.MyDataReader.GetValue(4).ToString()
                    fpPersonelList.Sheets(0).Cells(i, 4).Text = DbConn.MyDataReader.GetValue(5).ToString()
                    i = i + 1
                End While
                DbConn.Kapat()
            End If
            If GIRENUNVAN = "ŞEF" Then
                SQL = ""
                SQL = "SELECT IK.NUFUS.ID,IK.NUFUS.ADI,IK.NUFUS.SOYADI,IK.UNITESI.UNITE,IK.UNITESI.BOLUM,IK.UNITESI.ALT_GRUP " _
                & " FROM IK.NUFUS " _
                & " INNER JOIN IK.UNITESI ON IK.NUFUS.ID=IK.UNITESI.ID " _
                & " WHERE IK.UNITESI.BOLUM='" & GIRENBOLUM & "'" _
                & " AND IK.UNITESI.ID<>" & IKID _
                & " AND IK.UNITESI.DURUMU='AKTIF'" _
                & " AND IK.UNITESI.BOLUM<>'TAŞERON FIRMA'" _
                & " AND IK.NUFUS.DRM=" & 1 _
                & " AND (IK.UNITESI.ALT_GRUP NOT LIKE '%ŞEF%')"
                If drpBolumler.Text <> "(All)" And drpBolumler.Text <> "" Then
                    SQL = SQL & " AND IK.UNITESI.ALT_GRUP ='" & drpBolumler.Text & "'"
                End If

                'SEZGIN BEY KAYITLARDA SEF GOZUKMESEDE FIZIKCILERI SEF GIBI ONAYLAMASI ICIN
                If IKID = 1001117 Then
                    SQL = ""
                    SQL = "SELECT IK.NUFUS.ID,IK.NUFUS.ADI,IK.NUFUS.SOYADI,IK.UNITESI.UNITE,IK.UNITESI.BOLUM,IK.UNITESI.ALT_GRUP FROM IK.NUFUS INNER JOIN IK.UNITESI ON IK.NUFUS.ID=IK.UNITESI.ID " _
                    & " WHERE IK.UNITESI.UNITE='" & GIRENUNITE & "'" _
                    & " AND IK.UNITESI.ID<>" & IKID _
                    & " AND IK.NUFUS.ID<>" & 1001841 _
                    & " AND IK.UNITESI.DURUMU='AKTIF'" _
                    & " AND IK.UNITESI.BOLUM<>'TAŞERON FIRMA'" _
                    & " AND IK.UNITESI.BOLUM<>'TEL-ÇUBUK HH K_GÜVENCE ŞEFLİĞİ'" _
                    & " AND (IK.UNITESI.ALT_GRUP NOT LIKE '%MÜHENDİS%')" _
                    & " AND IK.NUFUS.DRM=" & 1
                    If drpBolumler.Text <> "(All)" And drpBolumler.Text <> "" Then
                        SQL = SQL & " AND IK.UNITESI.ALT_GRUP ='" & drpBolumler.Text & "'"
                    End If
                End If

                Dim DbConn1 As New ConnectOracleDilerIK
                DbConn1.RaporWhile(SQL)
                While DbConn1.MyDataReader.Read
                    fpPersonelList.Sheets(0).RowCount = fpPersonelList.Sheets(0).RowCount + 1
                    fpPersonelList.Sheets(0).Cells(i, 0).Text = DbConn1.MyDataReader.GetValue(0).ToString()
                    fpPersonelList.Sheets(0).Cells(i, 1).Text = DbConn1.MyDataReader.GetValue(1).ToString() & " " & DbConn1.MyDataReader.GetValue(2).ToString()
                    fpPersonelList.Sheets(0).Cells(i, 2).Text = DbConn1.MyDataReader.GetValue(3).ToString()
                    fpPersonelList.Sheets(0).Cells(i, 3).Text = DbConn1.MyDataReader.GetValue(4).ToString()
                    fpPersonelList.Sheets(0).Cells(i, 4).Text = DbConn1.MyDataReader.GetValue(5).ToString()
                    i = i + 1
                End While
                DbConn1.Kapat()
            End If

            If GIRENUNVAN = "MÜHENDİSİ" Then
                SQL = ""
                SQL = "SELECT IK.NUFUS.ID,IK.NUFUS.ADI,IK.NUFUS.SOYADI,IK.UNITESI.UNITE,IK.UNITESI.BOLUM,IK.UNITESI.ALT_GRUP FROM IK.NUFUS INNER JOIN IK.UNITESI ON IK.NUFUS.ID=IK.UNITESI.ID " _
                & " WHERE IK.UNITESI.BOLUM='" & GIRENBOLUM & "'" _
                & " AND IK.UNITESI.ID<>" & IKID _
                 & " AND IK.UNITESI.BOLUM<>'TAŞERON FIRMA'" _
                & " AND IK.NUFUS.DRM=" & 1 _
                & " AND IK.UNITESI.DURUMU='AKTIF'" _
                & " AND (IK.UNITESI.ALT_GRUP NOT LIKE '%ŞEF%')" _
                & " AND (IK.UNITESI.ALT_GRUP NOT LIKE '%MÜHENDİS%')"
                If drpBolumler.Text <> "(All)" And drpBolumler.Text <> "" Then
                    SQL = SQL & " AND IK.UNITESI.ALT_GRUP ='" & drpBolumler.Text & "'"
                End If


                If IKID = 1002587 Then
                    SQL = ""
                    SQL = "SELECT IK.NUFUS.ID,IK.NUFUS.ADI,IK.NUFUS.SOYADI,IK.UNITESI.UNITE,IK.UNITESI.BOLUM,IK.UNITESI.ALT_GRUP FROM IK.NUFUS INNER JOIN IK.UNITESI ON IK.NUFUS.ID=IK.UNITESI.ID " _
                    & " WHERE IK.UNITESI.UNITE='" & GIRENUNITE & "'" _
                    & " AND IK.UNITESI.ID<>" & IKID _
                    & " AND IK.NUFUS.ID<>" & 1001117 _
                    & " AND IK.NUFUS.ID<>" & 1001841 _
                    & " AND IK.UNITESI.DURUMU='AKTIF'" _
                    & " AND IK.UNITESI.BOLUM<>'TAŞERON FIRMA'" _
                    & " AND (IK.UNITESI.ALT_GRUP NOT LIKE '%ŞEF%')" _
                    & " AND (IK.UNITESI.ALT_GRUP NOT LIKE '%MÜHENDİS%')" _
                    & " AND IK.NUFUS.DRM=" & 1
                    If drpBolumler.Text <> "(All)" And drpBolumler.Text <> "" Then
                        SQL = SQL & " AND IK.UNITESI.ALT_GRUP ='" & drpBolumler.Text & "'"
                    End If
                End If


                'SQL = ""
                'SQL = "SELECT IK.NUFUS.ID,IK.NUFUS.ADI,IK.NUFUS.SOYADI,IK.UNITESI.UNITE,IK.UNITESI.BOLUM,IK.UNITESI.ALT_GRUP FROM IK.NUFUS INNER JOIN IK.UNITESI ON IK.NUFUS.ID=IK.UNITESI.ID " _
                '& " WHERE IK.UNITESI.BOLUM='" & GIRENBOLUM & "'" _
                '& " AND IK.UNITESI.ID<>" & IKID _
                '& " AND IK.NUFUS.DRM=" & 1
                'If DropDownList1.Text <> "(All)" And DropDownList1.Text <> "" Then
                '    SQL = SQL & " AND IK.UNITESI.ALT_GRUP ='" & DropDownList1.Text & "'"
                'End If
                Dim DbConn2 As New ConnectOracleDilerIK
                DbConn2.RaporWhile(SQL)
                While DbConn2.MyDataReader.Read
                    fpPersonelList.Sheets(0).RowCount = fpPersonelList.Sheets(0).RowCount + 1
                    fpPersonelList.Sheets(0).Cells(i, 0).Text = DbConn2.MyDataReader.GetValue(0).ToString()
                    fpPersonelList.Sheets(0).Cells(i, 1).Text = DbConn2.MyDataReader.GetValue(1).ToString() & " " & DbConn2.MyDataReader.GetValue(2).ToString()
                    fpPersonelList.Sheets(0).Cells(i, 2).Text = DbConn2.MyDataReader.GetValue(3).ToString()
                    fpPersonelList.Sheets(0).Cells(i, 3).Text = DbConn2.MyDataReader.GetValue(4).ToString()
                    fpPersonelList.Sheets(0).Cells(i, 4).Text = DbConn2.MyDataReader.GetValue(5).ToString()
                    i = i + 1
                End While
                DbConn2.Kapat()
            End If
            'fpPersonelList.Sheets(0).RowCount = fpPersonelList.Sheets(0).RowCount + 2
            'AKTIF_DONEM_NOTU_GETIR()
            'ONAYLADIM_MI()
            OnaylanmayanEgitimSay()
        End Sub
        Private Sub OnaylanmayanEgitimSay()
            Dim DbConn As New ConnectOracleDilerIK
            Dim drm As Integer = 0 'Alınan eğitimin onay durumu. 0 ise onaylanmamış 1 ise mühendis 2 ise şef onaylamış 3 müdür 4 fabrika müdürü 99 ise eski eğitim
            Dim sayi As Integer = 0 'Kaç tane onaylanmamış eğitimi var.
            For index As Integer = 0 To fpPersonelList.Sheets(0).RowCount - 1
                sayi = 0
                SQL = "SELECT EONAY FROM IK.ALINAN_EGITIMLER " _
                    & " WHERE IK.ALINAN_EGITIMLER.ID=" & fpPersonelList.Sheets(0).Cells(index, 0).Text
                DbConn.RaporWhile(SQL)
                While DbConn.MyDataReader.Read
                    drm = DbConn.MyDataReader.Item("EONAY").ToString()
                    If GIRENUNVAN = "MÜHENDİSİ" And (drm = 0) Then
                        sayi += 1
                    ElseIf GIRENUNVAN = "ŞEF" And (drm < 2) Then
                        sayi += 1
                    ElseIf GIRENUNVAN = "MÜDÜR" And (drm < 3) Then
                        sayi += 1
                    ElseIf GIRENUNVAN = "FABRİKA MÜDÜRÜ" And (drm < 4) Then
                        sayi += 1
                    End If
                End While
                DbConn.Kapat()
                fpPersonelList.Sheets(0).Cells(index, 6).Text = sayi
                If sayi <> 0 And GIRENUNVAN = "MÜDÜR" Then
                    fpPersonelList.Sheets(0).Cells(index, 6).BackColor = Color.Red
                ElseIf sayi <> 0 And GIRENUNVAN = "MÜHENDİSİ" Then
                    fpPersonelList.Sheets(0).Cells(index, 6).BackColor = Color.Red
                ElseIf sayi <> 0 And GIRENUNVAN = "MÜDÜR" Then
                    fpPersonelList.Sheets(0).Cells(index, 6).BackColor = Color.Red
                Else
                    fpPersonelList.Sheets(0).Cells(index, 6).BackColor = Color.Transparent
                End If
            Next
        End Sub
        Protected Sub FpSpread2_Grouped(ByVal sender As Object, ByVal e As System.EventArgs) Handles fpPersonelList.Grouped
            Dim ss As FarPoint.Web.Spread.FpSpread = sender
            Dim gm As FarPoint.Web.Spread.Model.GroupDataModel
            If TypeOf (ss.ActiveSheetView.DataModel) Is FarPoint.Web.Spread.Model.GroupDataModel Then
                gm = ss.ActiveSheetView.DataModel
                If TypeOf (gm.GroupComparer) Is MyGroupComparer Then
                    Dim i As Integer
                    For i = 0 To ss.ActiveSheetView.RowCount - 1
                        If gm.IsGroup(i) Then
                            Dim g As FarPoint.Web.Spread.Model.Group
                            g = gm.GetGroup(i)
                            If TypeOf (g.Rows(0)) Is Integer Then
                                Dim r As Integer = ss.ActiveSheetView.ColumnHeaderAutoTextIndex
                                Dim s As String = ss.ActiveSheetView.GetColumnLabel(r, ss.ActiveSheetView.GetViewColumnFromModelColumn(g.Column))
                                If s.IndexOf("Birth") >= 0 Then
                                    g.Text = s & ": " & GetDecade(gm.TargetModel.GetValue(g.Rows(0), g.Column)) & "s"
                                Else
                                    g.Text = s & ": " & GetYear(gm.TargetModel.GetValue(g.Rows(0), g.Column))
                                End If
                            End If
                        End If
                    Next
                End If

            End If
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            'Paint the commandbar background
            Dim cmdbar As WebControl = fpPersonelList.FindControl("commandBar")
            If Not cmdbar Is Nothing Then
                cmdbar.Attributes.CssStyle.Add("background-image", "url(images/ajax/toolbar.Horizontal.background.gif)")
            End If

            'Paint the 'Group' buttons
            Dim wc As WebControl = fpPersonelList.FindControl("groupBar")
            If Not wc Is Nothing Then
                Dim c1 As WebControl
                Dim i As Integer
                For i = 0 To wc.Controls.Count - 1
                    If i Mod 2 = 0 Then
                        'If even control, add the image (don't paint the 'links' image)
                        If TypeOf (wc.Controls(i)) Is WebControl Then
                            c1 = wc.Controls(i)
                            c1.Attributes.CssStyle.Add("background-image", "url(images/ajax/toolbar.Horizontal.background.gif)")
                        End If
                    End If
                Next
            End If

            MyBase.Render(writer)
        End Sub

        Private Sub YoneticimiKontrolu()
            'FpSpread1.Sheets(4).Visible = True
            Dim Neki As Int16
            SQL = "SELECT IK.FABRIKAGENEL.YONETICILIK FROM IK.FABRIKAGENEL WHERE IK.FABRIKAGENEL.ID=" & IKDegerlendirilenID
            Dim DbConn As New ConnectOracleDilerIK
            DbConn.RaporWhile(SQL)
            While DbConn.MyDataReader.Read
                Neki = DbConn.MyDataReader.GetValue(0)
            End While
            DbConn.Kapat()
            If Neki = 0 Then
                'FpSpread1.Sheets(4).Visible = False
                YoneticiDrm = "H"
            Else
                'FpSpread1.Sheets(4).Visible = True
                YoneticiDrm = "E"
            End If
        End Sub

        Public Sub TERSCEVIR(ByVal gelen As Double)
            DonenTersTarih1 = ""
            Dim YIL, AY, GUN As String
            YIL = Microsoft.VisualBasic.Left(gelen, 4)
            AY = Mid(gelen, 5, 2)
            GUN = Mid(gelen, 7, 2)
            DonenTersTarih1 = GUN & "/" & AY & "/" & YIL
        End Sub
        Public Sub CEVIR(ByVal gelen As Date)
            DonenTarih = ""
            Dim YIL, AY, GUN As String
            YIL = Microsoft.VisualBasic.Year(gelen)
            AY = Microsoft.VisualBasic.Month(gelen)
            If Microsoft.VisualBasic.Len(AY.ToString) = 1 Then AY = "0" & AY
            GUN = Microsoft.VisualBasic.Day(gelen)
            If Microsoft.VisualBasic.Len(GUN) = 1 Then GUN = "0" & GUN
            DonenTarih = YIL & AY & GUN
        End Sub

        Private Function GetYear(ByVal value As DateTime) As Integer
            Return value.Year
        End Function

        Private Function GetDecade(ByVal value As DateTime) As Integer
            Dim x As Integer = 0
            x = value.Year Mod 10
            x = value.Year - x
            Return x
        End Function

        Protected Sub DropDownList1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpBolumler.SelectedIndexChanged
            If GIRENUNVAN = "FABRİKA MÜDÜRÜ" Then
                FABRIKA_MUDURU_ICIN_GRIDDOLDUR()
            Else
                GIRENIN_ALTLARINI_GETIR()
            End If
        End Sub
        Private Sub FABRIKA_MUDURU_ICIN_GRIDDOLDUR()
            fpPersonelList.Sheets(0).RowCount = 0

            Dim i = 0
            SQL = ""
            SQL = "SELECT IK.NUFUS.ID,IK.NUFUS.ADI,IK.NUFUS.SOYADI,IK.UNITESI.UNITE,IK.UNITESI.BOLUM,IK.UNITESI.ALT_GRUP FROM IK.NUFUS INNER JOIN IK.UNITESI ON IK.NUFUS.ID=IK.UNITESI.ID " _
            & " WHERE IK.UNITESI.ID<>" & IKID _
            & " AND IK.NUFUS.DRM=" & 1
            If drpBolumler.Text <> "(All)" And drpBolumler.Text <> "" Then
                SQL = SQL & " AND IK.UNITESI.ALT_GRUP ='" & drpBolumler.Text & "'"
            End If
            Dim DbConn As New ConnectOracleDilerIK
            DbConn.RaporWhile(SQL)
            While DbConn.MyDataReader.Read
                fpPersonelList.Sheets(0).RowCount = fpPersonelList.Sheets(0).RowCount + 1
                fpPersonelList.Sheets(0).Cells(i, 0).Text = DbConn.MyDataReader.GetValue(0).ToString()
                fpPersonelList.Sheets(0).Cells(i, 1).Text = DbConn.MyDataReader.GetValue(1).ToString() & " " & DbConn.MyDataReader.GetValue(2).ToString()
                fpPersonelList.Sheets(0).Cells(i, 2).Text = DbConn.MyDataReader.GetValue(3).ToString()
                fpPersonelList.Sheets(0).Cells(i, 3).Text = DbConn.MyDataReader.GetValue(4).ToString()
                fpPersonelList.Sheets(0).Cells(i, 4).Text = DbConn.MyDataReader.GetValue(5).ToString()
                'FpSpread1.Sheets(0).Cells(i, 5).Text = DbConn.MyDataReader.GetValue(5).ToString()
                i = i + 1
            End While
            DbConn.Kapat()
            OnaylanmayanEgitimSay()

        End Sub

        Private Sub EgitimIseUygSecilenleriTemizle()
            EgitimIseUygulanmasi.SaveChanges()
            For index As Integer = 0 To EgitimIseUygulanmasi.Sheets(0).RowCount - 1
                EgitimIseUygulanmasi.Sheets(0).Cells(index, 1).ResetValue()
            Next
            EgitimIseUygulanmasi.Sheets(1).Cells(0, 2).ResetValue()
            EgitimIseUygulanmasi.Sheets(1).Cells(2, 2).ResetValue()
        End Sub

        Protected Sub Sakla_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles Sakla.Click
            If bosAlanKontrol() Then
                EgitimIseUygulanmasi.SaveChanges()
                GIRENUNVAN = Session("GIRENUNVAN") ' "MÜHENDİSİ" "ŞEF" "MÜDÜR"
                Dim onayDurum As Int16 ' 0 default - 1 mühendis - 2 şef - 3 müdür
                Select Case GIRENUNVAN
                    Case "MÜHENDİSİ"
                        onayDurum = 1
                    Case "ŞEF"
                        onayDurum = 2
                    Case "MÜDÜR"
                        onayDurum = 3
                    Case "FABRİKA MÜDÜRÜ"
                        onayDurum = 4
                End Select


                Dim onayTarih As Date
                onayTarih = DateTime.ParseExact(txtOnayTar.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                CEVIR(onayTarih)
                Dim strSecilenEgt() As String
                strSecilenEgt = Split(ListBox1.SelectedItem.Text.ToUpper, "-")
                Dim alinanEgitimNo As Integer 'Kişinin kaçıncı eğitimi aldığını tutan değer
                alinanEgitimNo = strSecilenEgt(0)
                Dim egitimKonu As String
                egitimKonu = Mid(ListBox1.SelectedItem.Text.ToUpper, alinanEgitimNo.ToString.Length + 2)

                If egitimKonu <> "ORYANTASYON" Then
                    Hesapla()
                    SQL = " UPDATE ALINAN_EGITIMLER SET " _
                    & " P1=" & IIf(EgitimIseUygulanmasi.Sheets(0).Rows(0).Visible, IIf(EgitimIseUygulanmasi.Sheets(0).Cells(0, 1).Value Is Nothing, 0, EgitimIseUygulanmasi.Sheets(0).Cells(0, 1).Value), 0) & "," _
                    & " P2=" & IIf(EgitimIseUygulanmasi.Sheets(0).Rows(1).Visible, IIf(EgitimIseUygulanmasi.Sheets(0).Cells(1, 1).Value Is Nothing, 0, EgitimIseUygulanmasi.Sheets(0).Cells(1, 1).Value), 0) & "," _
                    & " P3=" & IIf(EgitimIseUygulanmasi.Sheets(0).Rows(2).Visible, IIf(EgitimIseUygulanmasi.Sheets(0).Cells(2, 1).Value Is Nothing, 0, EgitimIseUygulanmasi.Sheets(0).Cells(2, 1).Value), 0) & "," _
                    & " P4=" & IIf(EgitimIseUygulanmasi.Sheets(0).Rows(3).Visible, IIf(EgitimIseUygulanmasi.Sheets(0).Cells(3, 1).Value Is Nothing, 0, EgitimIseUygulanmasi.Sheets(0).Cells(3, 1).Value), 0) & "," _
                    & " P5=" & IIf(EgitimIseUygulanmasi.Sheets(0).Rows(4).Visible, IIf(EgitimIseUygulanmasi.Sheets(0).Cells(4, 1).Value Is Nothing, 0, EgitimIseUygulanmasi.Sheets(0).Cells(4, 1).Value), 0) & "," _
                    & " P6=" & IIf(EgitimIseUygulanmasi.Sheets(0).Rows(5).Visible, IIf(EgitimIseUygulanmasi.Sheets(0).Cells(5, 1).Value Is Nothing, 0, EgitimIseUygulanmasi.Sheets(0).Cells(5, 1).Value), 0) & "," _
                    & " P7=" & IIf(EgitimIseUygulanmasi.Sheets(0).Rows(6).Visible, IIf(EgitimIseUygulanmasi.Sheets(0).Cells(6, 1).Value Is Nothing, 0, EgitimIseUygulanmasi.Sheets(0).Cells(6, 1).Value), 0) & "," _
                    & " P8=" & IIf(EgitimIseUygulanmasi.Sheets(0).Rows(7).Visible, IIf(EgitimIseUygulanmasi.Sheets(0).Cells(7, 1).Value Is Nothing, 0, EgitimIseUygulanmasi.Sheets(0).Cells(7, 1).Value), 0) & "," _
                    & " EDEGER=" & EgitimIseUygulanmasi.Sheets(1).Cells(2, 2).Value & "," _
                    & " TOPLAMPUAN=" & EgitimIseUygulanmasi.Sheets(0).Cells(8, 1).Value & "," _
                    & " ONAYTARIH=" & DonenTarih & "," _
                    & " EONAY=" & onayDurum & "" _
                    & " WHERE ID=" & txtSicilNo.Text _
                    & " AND EGITIM_TANIMI='" & egitimKonu & "'" _
                    & " AND SIRA_NO=" & strSecilenEgt(0)
                Else
                    SQL = " UPDATE ALINAN_EGITIMLER SET " _
                    & " TOPLAMPUAN=" & EgitimIseUygulanmasi.Sheets(1).Cells(0, 2).Value & "," _
                    & " ONAYTARIH=" & DonenTarih & "," _
                    & " EONAY=" & onayDurum & "" _
                    & " WHERE ID=" & txtSicilNo.Text _
                    & " AND EGITIM_TANIMI='" & egitimKonu & "'" _
                    & " AND SIRA_NO=" & strSecilenEgt(0)
                End If

                Dim DbConn As New ConnectOracleDilerIK
                DbConn.SaveUpdate(SQL)
                DbConn.Kapat()
            Else
                AspNetMsgFunc("Boş alan bulunmaktadır. III. ve IV. Bölümleri Kontrol ediniz")
            End If
            OnaylanmamisEgitimGetir(txtSicilNo.Text)
            OnaylanmayanEgitimSay()
        End Sub

        Private Function bosAlanKontrol()
            ''Kontrol edilecek
            Dim bosalan As Boolean = False
            'Dim sayacSatir = 0
            'Dim sayacIsaretlenenCevap = 0
            EgitimIseUygulanmasi.SaveChanges()
            Dim strSecilenEgt() As String
            strSecilenEgt = Split(ListBox1.SelectedItem.Text.ToUpper, "-")
            Dim alinanEgitimNo As Integer 'Kişinin kaçıncı eğitimi aldığını tutan değer
            alinanEgitimNo = strSecilenEgt(0)
            Dim egitimKonu As String
            egitimKonu = Mid(ListBox1.SelectedItem.Text.ToUpper, alinanEgitimNo.ToString.Length + 2)
            If egitimKonu = "ORYANTASYON" Then
                '    If EgitimIseUygulanmasi.Sheets(1).Cells(0, 2).Value Is Null Or EgitimIseUygulanmasi.Sheets(1).Cells(0, 2).Value Is Nothing Then
                '        bosalan = True
                '    End If
            Else
                '    For index As Integer = 0 To EgitimIseUygulanmasi.Sheets(0).RowCount - 1
                '        If EgitimIseUygulanmasi.Sheets(0).Rows(index).Visible = False Then
                '            sayacSatir += 1
                '        End If
                '    Next
                '    For index As Integer = 0 To EgitimIseUygulanmasi.Sheets(0).RowCount - 1
                '        If EgitimIseUygulanmasi.Sheets(0).Cells(index, 1).Value IsNot Null Or EgitimIseUygulanmasi.Sheets(0).Cells(index, 1).Value IsNot Nothing Then
                '            sayacIsaretlenenCevap += 1 'Burası hatalı, satır sayısı ile girilen değerleri kontrol edecez
                '        End If
                '    Next
                '    If sayacSatir <> sayacIsaretlenenCevap Then
                '        bosalan = True
                '    End If
            End If
            If bosalan Then
                Return 0
            Else
                Return 1
            End If
        End Function
        Private Sub ONAYLADIM_MI()
            'Try
            '    SQL = ""
            '    SQL = "SELECT * FROM PERFORMANS_DONEM"
            '    Dim DbConn2 As New ConnectOracleDilerIK
            '    DbConn2.RaporWhile(SQL)
            '    While DbConn2.MyDataReader.Read
            '        YilX = DbConn2.MyDataReader.GetValue(0).ToString()
            '        DonemX = DbConn2.MyDataReader.GetValue(1).ToString()
            '    End While
            '    DbConn2.Kapat()
            '    IKDegerlendirilenID = ""
            '    Dim k
            '    For k = 0 To fpPersonelList.Sheets(0).RowCount - 1
            '        IKDegerlendirilenID = fpPersonelList.Sheets(0).Cells(k, 1).Text
            '        SQL = ""
            '        SQL = "SELECT COUNT(PUANVEREN) FROM PERFORMANS_DATA " _
            '        & " WHERE ID=" & IKDegerlendirilenID _
            '        & " AND YIL=" & YilX _
            '        & " AND DONEM=" & DonemX _
            '        & " AND PUANVEREN=" & KayitYapan

            '        Dim DbConn3 As New ConnectOracleDilerIK
            '        DbConn3.Sayac(SQL)
            '        Dim SAYAC = DbConn3.GeriDonenRakam
            '        DbConn3.Kapat()
            '        If SAYAC > 0 Then
            '            fpPersonelList.Sheets(0).Cells(k, 0).BackColor = Color.LightCyan
            '            fpPersonelList.Sheets(0).Cells(k, 1).BackColor = Color.LightCyan
            '            fpPersonelList.Sheets(0).Cells(k, 2).BackColor = Color.LightCyan
            '            fpPersonelList.Sheets(0).Cells(k, 3).BackColor = Color.LightCyan
            '            fpPersonelList.Sheets(0).Cells(k, 4).BackColor = Color.LightCyan
            '            'FpSpread2.Sheets(0).Cells(k, 6).BackColor = Color.YellowGreen
            '        End If
            '        'Dim DbConn As New ConnectOracleDilerIK
            '        ''DbConn.Sayac(SQL)

            '        'DbConn.RaporWhile(SQL)
            '        'While DbConn.MyDataReader.Read
            '        '    FpSpread2.Sheets(0).Cells(k, 7).Text = KayitYapan

            '        'End While
            '        'DbConn.Kapat()
            '    Next
            'Catch ex As Exception

            'End Try

        End Sub

        Protected Sub ImageButton3_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton3.Click
            Hesapla()
        End Sub
        Private Sub Hesapla()
            EgitimIseUygulanmasi.SaveChanges()
            Dim toplam As Decimal
            Dim sayac = 0
            For index As Integer = 0 To 7
                If EgitimIseUygulanmasi.Sheets(0).Cells(index, 1).Value <> 0 Then
                    sayac += 1
                End If
            Next
            Dim soruDegeri As Decimal = 0.0
            soruDegeri = 100 / (5 * sayac)
            For i As Integer = 0 To 7
                toplam += (EgitimIseUygulanmasi.Sheets(0).Cells(i, 1).Value * soruDegeri)
            Next
            EgitimIseUygulanmasi.Sheets(0).Cells(8, 1).Text = toplam
            If toplam < 60 Then
                EgitimIseUygulanmasi.Sheets(0).Cells(8, 1).BackColor = Color.Red
            Else
                EgitimIseUygulanmasi.Sheets(0).Cells(8, 1).BackColor = Color.Transparent
            End If
        End Sub

        Protected Sub ImageButton5_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton5.Click
            Response.Redirect("../../default2.aspx")
        End Sub
        'Protected Sub drpAy_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpAy.SelectedIndexChanged
        '    gunSayisiKontrol()
        'End Sub     

        Protected Sub fpPersonelList_ButtonCommand(ByVal sender As Object, ByVal e As FarPoint.Web.Spread.SpreadCommandEventArgs) Handles fpPersonelList.ButtonCommand
            EgitimIseUygSecilenleriTemizle()
            Dim row As Int32 = e.CommandArgument.X
            txtSicilNo.Text = e.SheetView.Cells(row, 0).Text.Trim()
            txtAdSoyad.Text = e.SheetView.Cells(row, 1).Text.Trim()
            txtUnite.Text = e.SheetView.Cells(row, 2).Text.Trim()
            txtBirim.Text = e.SheetView.Cells(row, 3).Text.Trim()
            txtBolum.Text = e.SheetView.Cells(row, 4).Text.Trim()
            OnaylanmamisEgitimGetir(e.SheetView.Cells(row, 0).Text.Trim())
            puanlaritemizle()
        End Sub
        Private Sub puanlaritemizle()
            txtReaksiyonOlc.Text = ""
            txtEgtOncTestSon.Text = ""
            txtEgtSonTestSon.Text = ""
            txtEgtTarih.Text = ""
        End Sub
        Private Sub OnaylanmamisEgitimGetir(ByVal gelensicil)
            ListBox1.Items.Clear()
            SQL = "SELECT SIRA_NO,EGITIM_TANIMI,EONAY FROM ALINAN_EGITIMLER " _
                & " WHERE ID=" & gelensicil
            Dim DbConn As New ConnectOracleDilerIK
            DbConn.RaporWhile(SQL)
            While DbConn.MyDataReader.Read
                If GIRENUNVAN = "MÜHENDİSİ" Then
                    If DbConn.MyDataReader.Item("EONAY").ToString() = 0 Then
                        ListBox1.Items.Add(DbConn.MyDataReader.Item("SIRA_NO").ToString() & "-" & DbConn.MyDataReader.Item("EGITIM_TANIMI").ToString()) 'Seçilen eğitimin kaçıncı eğitim olduğunu yazdırıyorum.
                    End If
                ElseIf GIRENUNVAN = "ŞEF" Then
                    If DbConn.MyDataReader.Item("EONAY").ToString() < 2 Then
                        ListBox1.Items.Add(DbConn.MyDataReader.Item("SIRA_NO").ToString() & "-" & DbConn.MyDataReader.Item("EGITIM_TANIMI").ToString())
                    End If
                ElseIf GIRENUNVAN = "MÜDÜR" Then
                    If DbConn.MyDataReader.Item("EONAY").ToString() < 3 Then
                        ListBox1.Items.Add(DbConn.MyDataReader.Item("SIRA_NO").ToString() & "-" & DbConn.MyDataReader.Item("EGITIM_TANIMI").ToString())
                    End If
                ElseIf GIRENUNVAN = "FABRİKA MÜDÜRÜ" Then
                    If DbConn.MyDataReader.Item("EONAY").ToString() < 4 Then
                        ListBox1.Items.Add(DbConn.MyDataReader.Item("SIRA_NO").ToString() & "-" & DbConn.MyDataReader.Item("EGITIM_TANIMI").ToString())
                    End If
                End If

            End While
            DbConn.Kapat()
        End Sub
        Protected Sub ListBox1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
            puanlaritemizle()
            EgitimIseUygSecilenleriTemizle()
            Dim strSecilenEgt() As String
            strSecilenEgt = Split(ListBox1.SelectedItem.Text.ToUpper, "-")
            Dim alinanEgitimNo As Integer 'Kişinin kaçıncı eğitimi aldığını tutan değer
            alinanEgitimNo = strSecilenEgt(0)
            Dim egitimKonu As String
            egitimKonu = Mid(ListBox1.SelectedItem.Text.ToUpper, alinanEgitimNo.ToString.Length + 2)


            Dim icerikNo() As String 'Eğitimin içerik numarası
            Dim SQL1 As String
            SQL1 = "SELECT ICERIK_NO FROM EGITIM_AYRINTI " _
            & " WHERE EGITIM_KONU='" & egitimKonu & "'"
            Dim DbConn As New ConnectOracleDilerIK
            DbConn.RaporWhile(SQL1)
            While DbConn.MyDataReader.Read
                icerikNo = DbConn.MyDataReader.Item("ICERIK_NO").ToString().Split("-")
            End While
            DbConn.Kapat()

            SQL = "SELECT BASLANGIC_TARIHI,EGT_ONCESI_NOT,EGT_SONRASI_NOT,REAKSIYON_OLC FROM IK.ALINAN_EGITIMLER " _
            & " WHERE ID='" & txtSicilNo.Text & "'" _
            & " AND SIRA_NO='" & strSecilenEgt(0) & "'"
            DbConn.RaporWhile(SQL)
            While DbConn.MyDataReader.Read
                TERSCEVIR(DbConn.MyDataReader.Item("BASLANGIC_TARIHI").ToString())
                txtEgtTarih.Text = DonenTersTarih1
                txtEgtOncTestSon.Text = DbConn.MyDataReader.Item("EGT_ONCESI_NOT").ToString()
                txtEgtSonTestSon.Text = DbConn.MyDataReader.Item("EGT_SONRASI_NOT").ToString()
                txtReaksiyonOlc.Text = DbConn.MyDataReader.Item("REAKSIYON_OLC").ToString()
            End While
            DbConn.Kapat()


            If strSecilenEgt(1).ToUpper = "ORYANTASYON" Then
                EgitimIseUygulanmasi.Sheets(0).Visible = False
                EgitimIseUygulanmasi.Sheets(1).Rows(2).Visible = False
                EgitimIseUygulanmasi.Sheets(1).Rows(3).Visible = False
                EgitimIseUygulanmasi.Sheets(1).Rows(0).Visible = True
                EgitimIseUygulanmasi.Sheets(1).Rows(1).Visible = True
            Else

                'Dim strdiz() As String
                'Dim icerikNo() As String 'Eğitimin içerik numarası
                'SQL = "SELECT ICERIK_NO FROM EGITIM_AYRINTI " _
                '& " WHERE EGITIM_KONU='" & egitimKonu & "'"
                'Dim DbConn As New ConnectOracleDilerIK
                'DbConn.RaporWhile(SQL)
                'While DbConn.MyDataReader.Read
                '    icerikNo = DbConn.MyDataReader.Item("ICERIK_NO").ToString().Split("-")
                'End While
                'DbConn.Kapat()

                'SQL = "SELECT BASLANGIC_TARIHI,EGT_ONCESI_NOT,EGT_SONRASI_NOT,REAKSIYON_OLC FROM ALINAN_EGITIMLER " _
                '& " WHERE ID='" & txtSicilNo.Text & "'" _
                '& " AND SIRA_NO='" & strSecilenEgt(0) & "'"
                'DbConn.RaporWhile(SQL)
                'While DbConn.MyDataReader.Read
                '    TERSCEVIR(DbConn.MyDataReader.Item("BASLANGIC_TARIHI").ToString())
                '    txtEgtTarih.Text = DonenTersTarih1
                '    txtEgtOncTestSon.Text = DbConn.MyDataReader.Item("EGT_ONCESI_NOT").ToString()
                '    txtEgtSonTestSon.Text = DbConn.MyDataReader.Item("EGT_SONRASI_NOT").ToString()
                '    txtReaksiyonOlc.Text = DbConn.MyDataReader.Item("REAKSIYON_OLC").ToString()
                'End While
                'DbConn.Kapat()

                For index As Integer = 0 To EgitimIseUygulanmasi.Sheets(0).RowCount - 2
                    EgitimIseUygulanmasi.Sheets(0).Rows(index).Visible = False
                Next
                For index1 As Integer = 0 To 7
                    For index As Integer = 0 To icerikNo.Length - 1
                        If icerikNo(index).Contains(index1.ToString) Then
                            EgitimIseUygulanmasi.Sheets(0).Rows(index1).Visible = True
                        End If
                    Next
                Next
                EgitimIseUygulanmasi.Sheets(0).Visible = True
                EgitimIseUygulanmasi.Sheets(1).Rows(2).Visible = True
                EgitimIseUygulanmasi.Sheets(1).Rows(3).Visible = True
                EgitimIseUygulanmasi.Sheets(1).Rows(0).Visible = False
                EgitimIseUygulanmasi.Sheets(1).Rows(1).Visible = False
            End If
        End Sub

        Protected Sub imgAltGrupGetir_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgAltGrupGetir.Click
            Dim alt_grup As String
            Dim DbConn As New ConnectOracleDilerIK
            SQL = "SELECT ALT_GRUP FROM IK.UNITESI " _
            & " WHERE DURUMU='AKTIF'" _
            & " AND ID='" & txtSicilNo.Text & "'"

            DbConn.RaporWhile(SQL)
            While DbConn.MyDataReader.Read
                alt_grup = DbConn.MyDataReader.Item("ALT_GRUP").ToString()
            End While
            DbConn.Kapat()

            drpBolumler.SelectedValue = alt_grup
            DropDownList1_SelectedIndexChanged("a", e)
        End Sub
    End Class

    Public Class MyGroupComparer
        Implements IComparer

        Private birthDate As Boolean = True
        Public Sub New(ByVal bd As Boolean)
            birthDate = bd
        End Sub

        Public Function Compare(ByVal x1 As Object, ByVal y1 As Object) As Integer Implements System.Collections.IComparer.Compare

            Dim x, y As Integer
            x = 0
            y = 0

            If birthDate Then
                If TypeOf (x1) Is DateTime Then
                    x = CType(x1, DateTime).Year Mod 10
                    x = CType(x1, DateTime).Year - x
                End If
                If TypeOf (y1) Is DateTime Then
                    y = CType(y1, DateTime).Year Mod 10
                    y = CType(y1, DateTime).Year - y
                End If
            Else
                If TypeOf (x1) Is DateTime Then
                    x = CType(x1, DateTime).Year
                End If
                If TypeOf (y1) Is DateTime Then
                    y = CType(y1, DateTime).Year
                End If
            End If

            If x = y Then
                Return 0
            ElseIf x > y Then
                Return 1
            Else
                Return -1
            End If
        End Function
    End Class



    Friend Class ConnectOracleDilerIK

        Public datasource As String
        Public username As String
        Public password As String
        Public conn As New OracleConnection()
        Public cmd As New OracleCommand
        Public da As New OracleDataAdapter
        Public ds As New DataSet
        Public dr As DataRow
        Public MyDataReader As OracleDataReader
        Public MyDataReader2 As OracleDataReader
        Public GeriDonenRakam As Double
        Public GeriDonenString As String

        Public Sub DbBaglan()
            Try
                Dim connectionString As String = "Data Source=(DESCRIPTION=" _
                & "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.198.191)(PORT=1521)))" _
                & "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=DLRORA)));" _
                & "User Id=IK;Password=IK;"
                conn = (New OracleConnection(connectionString))
                cmd.Connection = conn
                conn.Open()
            Catch
            End Try
        End Sub


        Public Sub RaporWhile(ByVal GelenTxt As String)

            DbBaglan()
            Dim ObjRS1 As New OracleCommand(GelenTxt, cmd.Connection)
            MyDataReader = ObjRS1.ExecuteReader()

        End Sub


        Public Sub Sil(ByVal GelenTxt As String)

            Try

                DbBaglan()
                Dim ObjRS1 As New OracleCommand(GelenTxt, cmd.Connection)
                ObjRS1.ExecuteNonQuery()
                ObjRS1.Connection.Close()

            Catch ex As Exception

            End Try

        End Sub
        Public Sub Sayac(ByVal GelenTxt As String)
            Try


                DbBaglan()
                Dim ObjRS1 As New OracleCommand(GelenTxt, cmd.Connection)
                GeriDonenRakam = ObjRS1.ExecuteScalar
                ObjRS1.Connection.Close()

            Catch ex As Exception
                GeriDonenRakam = 0
            End Try

        End Sub
        Public Sub SaveUpdate(ByVal GelenTxt As String)
            DbBaglan()
            Dim ObjRS1 As New OracleCommand(GelenTxt, cmd.Connection)
            ObjRS1.ExecuteNonQuery()
            ObjRS1.Connection.Close()

        End Sub
        Public Sub Kapat()
            conn.Close()
        End Sub

    End Class
End Namespace