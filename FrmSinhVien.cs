using System; using System.Data; using System.Data.SqlClient; using System.Windows.Forms;
namespace Lab08_SQLCommand {
public class FrmSinhVien:Form {
 TextBox ma=new TextBox(), ten=new TextBox(), diem=new TextBox(), email=new TextBox(), tim=new TextBox();
 ComboBox khoa=new ComboBox(); DateTimePicker ngay=new DateTimePicker(); CheckBox nam=new CheckBox(); ListBox ds=new ListBox(); Label thongke=new Label();
 public FrmSinhVien(){ Text="LAB 8 - SQLCommand va SqlParameter"; Width=1000; Height=680; StartPosition=FormStartPosition.CenterScreen; TaoGiaoDien(); Load+=(s,e)=>TaiKhoa(); }
 void LabelAt(string t,int x,int y){Controls.Add(new Label{Text=t,Left=x,Top=y+4,Width=100});}
 void ButtonAt(string t,int x,int y,EventHandler h){var b=new Button{Text=t,Left=x,Top=y,Width=150,Height=30};b.Click+=h;Controls.Add(b);}
 void TaoGiaoDien(){
  LabelAt("Ma sinh vien",20,20); ma.SetBounds(125,20,190,24);Controls.Add(ma);
  LabelAt("Ho ten",20,55);ten.SetBounds(125,55,190,24);Controls.Add(ten);
  LabelAt("Ngay sinh",20,90);ngay.SetBounds(125,90,190,24);Controls.Add(ngay);
  LabelAt("Gioi tinh nam",20,125);nam.SetBounds(125,125,20,24);Controls.Add(nam);
  LabelAt("Khoa",20,160);khoa.SetBounds(125,160,220,24);khoa.DropDownStyle=ComboBoxStyle.DropDownList;Controls.Add(khoa);
  LabelAt("Diem TB",20,195);diem.SetBounds(125,195,190,24);Controls.Add(diem);
  LabelAt("Email",20,230);email.SetBounds(125,230,190,24);Controls.Add(email);
  LabelAt("Tim/Xoa theo ma",20,265);tim.SetBounds(125,265,190,24);Controls.Add(tim);
  ButtonAt("1. Dem sinh vien",370,20,(s,e)=>Dem()); ButtonAt("2. Doc danh sach",535,20,(s,e)=>Doc());
  ButtonAt("3. Them sinh vien",700,20,(s,e)=>Them()); ButtonAt("4. Cap nhat diem",370,60,(s,e)=>CapNhat());
  ButtonAt("5. Tim theo khoa (SP)",535,60,(s,e)=>TheoKhoa()); ButtonAt("6. Tim theo ma",700,60,(s,e)=>TimMa());
  ButtonAt("7. Diem cao nhat",370,100,(s,e)=>DiemMax()); ButtonAt("8. Xoa co xac nhan",535,100,(s,e)=>Xoa());
  ButtonAt("9. Them + OUTPUT",700,100,(s,e)=>ThemOutput()); ButtonAt("10. Thu SqlHelper",370,140,(s,e)=>ThuHelper());
  thongke.SetBounds(535,145,400,30);Controls.Add(thongke);ds.SetBounds(20,320,930,290);Controls.Add(ds);
 }
 void TaiKhoa(){try{var t=SqlHelper.ExecuteTable("SELECT MaKhoa,TenKhoa FROM Khoa ORDER BY TenKhoa");khoa.DataSource=t;khoa.DisplayMember="TenKhoa";khoa.ValueMember="MaKhoa";}catch(Exception ex){MessageBox.Show("Loi tai khoa: "+ex.Message);}}
 bool HopLe(){decimal d;if(ma.Text.Trim()==""||ten.Text.Trim()==""||!decimal.TryParse(diem.Text,out d)||d<0||d>10||khoa.SelectedValue==null){MessageBox.Show("Nhap ma, ho ten, khoa va diem tu 0 den 10.");return false;}return true;}
 SqlParameter P(string n,SqlDbType t,int size,object v){var p=size>0?new SqlParameter(n,t,size):new SqlParameter(n,t);p.Value=v;return p;}
 void Dem(){var o=SqlHelper.ExecuteScalar("SELECT COUNT(*) FROM SinhVien");thongke.Text="Tong so sinh vien: "+Convert.ToInt32(o);}
 void Doc(){var t=SqlHelper.ExecuteTable("SELECT MaSV,HoTen,DiemTB FROM SinhVien ORDER BY HoTen");ds.Items.Clear();foreach(DataRow r in t.Rows)ds.Items.Add(r["MaSV"]+" - "+r["HoTen"]+" - "+Convert.ToDecimal(r["DiemTB"]).ToString("N2"));}
 void Them(){if(!HopLe())return;string q="INSERT INTO SinhVien(MaSV,HoTen,NgaySinh,GioiTinh,MaKhoa,DiemTB,Email) VALUES(@MaSV,@HoTen,@NgaySinh,@GioiTinh,@MaKhoa,@DiemTB,@Email)";
 using(var c=DbConnectionFactory.Create())using(var cmd=new SqlCommand(q,c)){cmd.Parameters.Add(P("@MaSV",SqlDbType.VarChar,15,ma.Text.Trim()));cmd.Parameters.Add(P("@HoTen",SqlDbType.NVarChar,100,ten.Text.Trim()));cmd.Parameters.Add(P("@NgaySinh",SqlDbType.Date,0,ngay.Value.Date));cmd.Parameters.Add(P("@GioiTinh",SqlDbType.Bit,0,nam.Checked));cmd.Parameters.Add(P("@MaKhoa",SqlDbType.VarChar,10,khoa.SelectedValue));var p=P("@DiemTB",SqlDbType.Decimal,0,decimal.Parse(diem.Text));p.Precision=4;p.Scale=2;cmd.Parameters.Add(p);cmd.Parameters.Add(P("@Email",SqlDbType.VarChar,100,email.Text.Trim()==""?(object)DBNull.Value:email.Text.Trim()));c.Open();MessageBox.Show("Da them "+cmd.ExecuteNonQuery()+" ban ghi.");Doc();}}
 void CapNhat(){decimal d;if(ma.Text.Trim()==""||!decimal.TryParse(diem.Text,out d)||d<0||d>10){MessageBox.Show("Nhap ma va diem hop le.");return;}using(var c=DbConnectionFactory.Create())using(var cmd=new SqlCommand("UPDATE SinhVien SET DiemTB=@DiemTB WHERE MaSV=@MaSV",c)){var p=P("@DiemTB",SqlDbType.Decimal,0,d);p.Precision=4;p.Scale=2;cmd.Parameters.Add(p);cmd.Parameters.Add(P("@MaSV",SqlDbType.VarChar,15,ma.Text.Trim()));c.Open();int n=cmd.ExecuteNonQuery();MessageBox.Show(n>0?"Cap nhat thanh cong":"Khong tim thay ma");}}
 void TheoKhoa(){using(var c=DbConnectionFactory.Create())using(var cmd=new SqlCommand("sp_SinhVien_TheoKhoa",c)){cmd.CommandType=CommandType.StoredProcedure;cmd.Parameters.Add(P("@MaKhoa",SqlDbType.VarChar,10,khoa.SelectedValue));c.Open();using(var r=cmd.ExecuteReader()){ds.Items.Clear();while(r.Read())ds.Items.Add(r["MaSV"]+" - "+r["HoTen"]+" - "+Convert.ToDecimal(r["DiemTB"]).ToString("N2"));}}}
 void TimMa(){using(var c=DbConnectionFactory.Create())using(var cmd=new SqlCommand("SELECT MaSV,HoTen,NgaySinh,DiemTB FROM SinhVien WHERE MaSV=@MaSV",c)){cmd.Parameters.Add(P("@MaSV",SqlDbType.VarChar,15,tim.Text.Trim()));c.Open();using(var r=cmd.ExecuteReader()){if(!r.Read()){MessageBox.Show("Khong tim thay sinh vien");return;}MessageBox.Show("Ma: "+r["MaSV"]+"\nHo ten: "+r["HoTen"]+"\nNgay sinh: "+Convert.ToDateTime(r["NgaySinh"]).ToShortDateString()+"\nDiem: "+r["DiemTB"]);}}}
 void DiemMax(){using(var c=DbConnectionFactory.Create())using(var cmd=new SqlCommand("SELECT MAX(DiemTB) FROM SinhVien WHERE MaKhoa=@MaKhoa",c)){cmd.Parameters.Add(P("@MaKhoa",SqlDbType.VarChar,10,khoa.SelectedValue));c.Open();object v=cmd.ExecuteScalar();MessageBox.Show(v==null||v==DBNull.Value?"Khoa chua co sinh vien":"Diem cao nhat: "+Convert.ToDecimal(v).ToString("N2"));}}
 void Xoa(){string id=tim.Text.Trim();if(id==""){MessageBox.Show("Nhap ma can xoa.");return;}if(MessageBox.Show("Xac nhan xoa "+id+"?","Xac nhan",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;try{int n=SqlHelper.ExecuteNonQuery("DELETE FROM SinhVien WHERE MaSV=@MaSV",P("@MaSV",SqlDbType.VarChar,15,id));MessageBox.Show(n>0?"Da xoa":"Khong tim thay ma");}catch(SqlException ex){MessageBox.Show("Khong xoa duoc, co the vi rang buoc khoa ngoai. "+ex.Message);}}
 void ThemOutput(){if(!HopLe())return;using(var c=DbConnectionFactory.Create())using(var cmd=new SqlCommand("sp_SinhVien_ThemVaDem",c)){cmd.CommandType=CommandType.StoredProcedure;cmd.Parameters.Add(P("@MaSV",SqlDbType.VarChar,15,ma.Text.Trim()));cmd.Parameters.Add(P("@HoTen",SqlDbType.NVarChar,100,ten.Text.Trim()));cmd.Parameters.Add(P("@NgaySinh",SqlDbType.Date,0,ngay.Value.Date));cmd.Parameters.Add(P("@GioiTinh",SqlDbType.Bit,0,nam.Checked));cmd.Parameters.Add(P("@MaKhoa",SqlDbType.VarChar,10,khoa.SelectedValue));var p=P("@DiemTB",SqlDbType.Decimal,0,decimal.Parse(diem.Text));p.Precision=4;p.Scale=2;cmd.Parameters.Add(p);cmd.Parameters.Add(P("@Email",SqlDbType.VarChar,100,email.Text.Trim()==""?(object)DBNull.Value:email.Text.Trim()));var o=cmd.Parameters.Add("@TongSinhVien",SqlDbType.Int);o.Direction=ParameterDirection.Output;c.Open();cmd.ExecuteNonQuery();MessageBox.Show("Tong sinh vien hien tai: "+o.Value);}}
 void ThuHelper(){Dem();Doc();}
}}