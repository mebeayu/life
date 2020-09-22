using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Life
{
    public partial class Form1 : Form
    {
        bool is_start = false;
        Bitmap bmp;
        Graphics gp;
        EntityMatrix em = new EntityMatrix();
        EntityMatrix em_b = new EntityMatrix();
        Thread thread_run;
        int cur_point_list = 1;

        int size_w = 168;
        int size_h = 98;
        int width = 10;
        int height = 10;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            width = this.ClientSize.Width / size_w;
            height = this.ClientSize.Height / size_h;

            em.InitSize(size_w, size_h,width,height);
            em_b.InitSize(size_w, size_h, width, height);
            InitView();
        }

        
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gp1 = e.Graphics;
            gp1.DrawImage(bmp, 0, 0);
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            Point point = this.PointToClient(Control.MousePosition);
            Entity entity = em.HitTest(point);
            if (entity != null)
            {
                entity.is_alive = !entity.is_alive;
                lock (Entity._lock)
                {
                    entity.Draw(gp);
                    this.Invalidate(entity.rect);
                }
                
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                if (is_start) return;
                is_start = true;
                thread_run = new Thread(Run);
                thread_run.Start();
                
            }
            else if (e.KeyCode == Keys.F2)
            {
                is_start = false;
                thread_run.Abort();
            }
            else if (e.KeyCode == Keys.F3)
            {
                if (is_start)
                {
                    is_start = false;
                    thread_run.Abort();
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Life文件|*.life";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, em.ToString());
                    }
                    is_start = true;
                    thread_run = new Thread(Run);
                    thread_run.Start();
                }
                else
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Life文件|*.life";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, em.ToString());
                    }
                }
            }
            else if (e.KeyCode == Keys.F4)
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "Life文件|*.life";
                
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (is_start)
                    {
                        is_start = false;
                        thread_run.Abort();
                    }
                    string data = File.ReadAllText(fileDialog.FileName);
                    em.LoadString(data);

                    size_w = em.SizeW;
                    size_h = em.SizeH;
                    width = this.ClientSize.Width / size_w;
                    height = this.ClientSize.Height / size_h;

                    
                    bmp = new Bitmap(em.SizeW * em.Width, em.SizeH * em.Height);
                    gp = Graphics.FromImage(bmp);
                    em.Draw(gp);
                    this.ClientSize = new Size(width * size_w, height * size_h);
                    this.Invalidate();

                }

            }
            else if (e.KeyCode == Keys.F5)
            {
                if (is_start)
                {
                    is_start = false;
                    thread_run.Abort();
                }
                File.WriteAllText(AppContext.BaseDirectory+"\\temp.life", em.ToString());
            }
            else if (e.KeyCode == Keys.F8)
            {
                if (is_start)
                {
                    is_start = false;
                    thread_run.Abort();
                }
                string data = File.ReadAllText(AppContext.BaseDirectory + "\\temp.life");
                em.LoadString(data);
                size_w = em.SizeW;
                size_h = em.SizeH;
                width = this.ClientSize.Width / size_w;
                height = this.ClientSize.Height / size_h;
                
                bmp = new Bitmap(em.SizeW * em.Width, em.SizeH * em.Height);
                gp = Graphics.FromImage(bmp);
                em.Draw(gp);
                this.ClientSize = new Size(width * size_w, height * size_h);
                this.Invalidate();
            }
            else if (e.KeyCode == Keys.F9)
            {
                em.ClearLife();
                em.Draw(gp);
                this.Invalidate();
            }
            else if (e.KeyCode==Keys.Left)
            {
                if (is_start)
                {
                    is_start = false;
                    thread_run.Abort();
                }
                em.MoveLeft();
                em.Draw(gp);
                this.Invalidate();
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (is_start)
                {
                    is_start = false;
                    thread_run.Abort();
                }
                em.MoveRight();
                em.Draw(gp);
                this.Invalidate();
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (is_start)
                {
                    is_start = false;
                    thread_run.Abort();
                }
                em.MoveUp();
                em.Draw(gp);
                this.Invalidate();
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (is_start)
                {
                    is_start = false;
                    thread_run.Abort();
                }
                em.MoveDown();
                em.Draw(gp);
                this.Invalidate();
            }
        }
        private void Run()
        {
            while (is_start)
            {
                for (int i = 0; i < em.SizeW; i++)
                {
                    for (int j = 0; j < em.SizeH; j++)
                    {
                        bool is_alive = em.ComputeEntityStatus(i, j);
                        em_b.SetEntityStatus(i, j, is_alive);
                    }
                }
                for (int i = 0; i < em.SizeW; i++)
                {
                    for (int j = 0; j < em.SizeH; j++)
                    {
                        if (em.Entitys[i,j].is_alive!= em_b.Entitys[i,j].is_alive)
                        {
                            em.Entitys[i, j].is_alive = em_b.Entitys[i, j].is_alive;
                            em.Entitys[i, j].Draw(gp);
                            this.Invalidate(em.Entitys[i, j].rect);
                        }
                    }

                }
                //this.Invalidate();
                Thread.Sleep(10);
            }
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (is_start)
            {
                is_start = false;
                thread_run.Abort();
            }
            
        }

        void InitView()
        {
            
            width = this.ClientSize.Width / size_w;
            height = this.ClientSize.Height / size_h;
            bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            em.Width = width;
            em.Height = height;
            gp = Graphics.FromImage(bmp);
            em.Draw(gp);
            this.ClientSize = new Size(width * size_w, height * size_h);
            this.Invalidate();
            
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (is_start)
            {
                is_start = false;
                thread_run.Abort();
            }
            if (WindowState == FormWindowState.Maximized)
            {
                InitView();
            }
            
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button==MouseButtons.Left)
            {
                Entity entity = em.HitTest(new Point(e.X,e.Y));
                if (entity != null)
                {
                    entity.is_alive = true;
                    lock (Entity._lock)
                    {
                        entity.Draw(gp);
                        this.Invalidate(entity.rect);
                    }

                }
            }
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            InitView();
            
        }
    }
}
