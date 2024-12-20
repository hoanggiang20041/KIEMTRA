using System;
using System.Linq;
using System.Windows.Forms;
using De01.Models;

namespace De01
{
    public partial class frmSinhvien : Form
    {
        private StudentService _studentService;
        private Sinhvien tempStudent;

        public frmSinhvien()
        {
            InitializeComponent();
            var context = new AppDbContext();
            var studentRepository = new StudentRepository(context);
            _studentService = new StudentService(studentRepository);
        }

        private void frmSinhvien_Load(object sender, EventArgs e)
        {
            LoadData();
            btnSave.Enabled = false;
            btnnotSave.Enabled = false;
            LoadClasses();
        }

        private void LoadData()
        {
            var sinhViens = _studentService.GetStudents();
            dataSv.DataSource = sinhViens.Select(sv => new
            {
                sv.MaSV,
                sv.HoTenSV,
                sv.NgaySinh,
                TenLop = sv.Lop.TenLop
            }).ToList();

            dataSv.Columns["MaSV"].HeaderText = "Mã SV";
            dataSv.Columns["HoTenSV"].HeaderText = "Họ và Tên";
            dataSv.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
            dataSv.Columns["TenLop"].HeaderText = "Lớp";

            dataSv.Columns["MaSV"].Width = 100;
            dataSv.Columns["HoTenSV"].Width = 150;
            dataSv.Columns["NgaySinh"].Width = 150;
            dataSv.Columns["TenLop"].Width = 150;
        }

        private void LoadClasses()
        {
            using (var context = new AppDbContext())
            {
                var classes = context.Lops.Select(l => new { l.MaLop, l.TenLop }).ToList();
                cmbClass.DataSource = classes;
                cmbClass.DisplayMember = "TenLop";
                cmbClass.ValueMember = "MaLop";
            }
        }

        private void dataSv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                var selectedRow = dataSv.Rows[e.RowIndex];
                txtMa.Text = selectedRow.Cells["MaSV"].Value.ToString();
                txtName.Text = selectedRow.Cells["HoTenSV"].Value.ToString();
                dtNgaysinh.Value = Convert.ToDateTime(selectedRow.Cells["NgaySinh"].Value);

                string tenLop = selectedRow.Cells["TenLop"].Value.ToString();
                using (var context = new AppDbContext())
                {
                    var maLop = context.Lops
                        .Where(l => l.TenLop == tenLop)
                        .Select(l => l.MaLop)
                        .FirstOrDefault();

                    cmbClass.SelectedValue = maLop;
                }

               
                btnSave.Enabled = true;
                btnnotSave.Enabled = true;
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string searchTerm = txtTim.Text.Trim();
            var result = _studentService.GetStudents().Where(sv => sv.HoTenSV.Contains(searchTerm)).ToList();

            if (result.Count > 0)
            {
                dataSv.DataSource = result.Select(sv => new
                {
                    sv.MaSV,
                    sv.HoTenSV,
                    sv.NgaySinh,
                    TenLop = sv.Lop.TenLop
                }).ToList();
            }
            else
            {
                MessageBox.Show("Không có sinh viên này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || cmbClass.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tempStudent = new Sinhvien
            {
                MaSV = txtMa.Text.Trim(),
                HoTenSV = txtName.Text.Trim(),
                NgaySinh = dtNgaysinh.Value,
                MaLop = (cmbClass.SelectedItem as dynamic).MaLop
            };

            _studentService.AddStudent(tempStudent);
            MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData();
            btnSave.Enabled = false;
            btnnotSave.Enabled = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMa.Text) || string.IsNullOrWhiteSpace(txtName.Text) || cmbClass.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn sinh viên để chỉnh sửa và điền đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tempStudent = _studentService.GetStudentById(txtMa.Text.Trim());

            if (tempStudent != null)
            {
                txtName.Text = tempStudent.HoTenSV;
                dtNgaysinh.Value = tempStudent.NgaySinh;
                cmbClass.SelectedValue = tempStudent.MaLop;

                btnSave.Enabled = true;
                btnnotSave.Enabled = true;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMa.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _studentService.DeleteStudent(txtMa.Text.Trim());
            MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (tempStudent != null)
            {
                tempStudent.HoTenSV = txtName.Text.Trim();
                tempStudent.NgaySinh = dtNgaysinh.Value;
                tempStudent.MaLop = (cmbClass.SelectedItem as dynamic).MaLop;

                _studentService.UpdateStudent(tempStudent);
                MessageBox.Show("Cập nhật thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                btnSave.Enabled = false;
                btnnotSave.Enabled = false;
            }
        }

        private void btnnotSave_Click(object sender, EventArgs e)
        {
            txtMa.Clear();
            txtName.Clear();
            dtNgaysinh.Value = DateTime.Now;
            cmbClass.SelectedIndex = -1;

            btnSave.Enabled = false;
            btnnotSave.Enabled = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void dtNgaysinh_ValueChanged(object sender, EventArgs e)
        {
            dtNgaysinh.CustomFormat = "dd/MM/yyyy";
            dtNgaysinh.Format = DateTimePickerFormat.Custom;
        }

      
    }
}