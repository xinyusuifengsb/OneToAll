using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace 一键启动
{
    public partial class Form1 : Form
    {
        private ImageList imageList = new ImageList();
        
        public Form1()
        {
            
            InitializeComponent();
            LoadSoftwarePaths(); // 主窗体构造函数，当窗体创建时会调用
            listView1.SmallImageList = imageList; // 关联ImageList
           // this.Load += new EventHandler(Form1_Load);// 在窗体加载时检查参数
        }



        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 添加软件路径到列表
                string filePath = openFileDialog.FileName;
                AddSoftwareToList(filePath);

                // 保存软件路径到本地设置或文件
                SaveSoftwarePaths();
            }
        }

        private void AddSoftwareToList(string path)
        {
            // 获取文件的图标
            Icon icon = Icon.ExtractAssociatedIcon(path);

            // 添加图标到ImageList
            imageList.Images.Add(path, icon.ToBitmap());

            // 添加项目到ListView
            ListViewItem item = new ListViewItem(Path.GetFileNameWithoutExtension(path), imageList.Images.IndexOfKey(path));
            item.Tag = path; // 用Tag属性保存完整路径
            listView1.Items.Add(item);
        }

        private void SaveSoftwarePaths()
        {
            // 在此处实现具体的保存逻辑，例如使用Settings或写入一个本地文件
            using (StreamWriter sw = new StreamWriter("path.txt"))
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    sw.WriteLine(item.Tag.ToString());
                }
            }
        }

        private void LoadSoftwarePaths()
        {
            if (File.Exists("path.txt"))
            {
                using (StreamReader sr = new StreamReader("path.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        AddSoftwareToList(line);
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs(); // 获取所有命令行参数
            if (args.Contains("-s"))
            {
                // 如果是静默模式，最小化窗口并从任务栏隐藏
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;

                // 自动触发startAllButton的点击事件
                button2_Click(this, EventArgs.Empty);
                Application.Exit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!File.Exists("path.txt"))
            {
                MessageBox.Show("没有找到软件路径文件！");
                return;
            }

            using (StreamReader sr = new StreamReader("path.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        Process.Start(line);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"无法启动指定路径的软件：{line}。错误：{ex.Message}");
                    }
                }
            }

            Application.Exit();

        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("确定要删除选中的项目吗？", "删除确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // 如果用户确认删除，则移除选中项
                    while (listView1.SelectedItems.Count > 0)
                    {
                        listView1.Items.Remove(listView1.SelectedItems[0]);
                    }

                    // 更新保存的路径
                    SaveSoftwarePaths();
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}
