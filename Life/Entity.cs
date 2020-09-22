using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life
{
    public class Entity
    {
        static public object _lock = new object();
        public int x { get; set; }
        public int y { get; set; }
        public bool is_alive { get; set; }
        public Rectangle rect { get; set; }
        public void Draw(Graphics e)
        {
            Pen pen = new Pen(Color.FromArgb(50, 50, 50));
            SolidBrush brush_dead = new SolidBrush(Color.Black);
            SolidBrush brush_alive = new SolidBrush(Color.Green);
            lock (_lock)
            {
                e.DrawRectangle(pen, rect);
                if (is_alive) e.FillRectangle(brush_alive, rect.X + 1, rect.Y + 1, rect.Width - 1, rect.Height - 1);
                else e.FillRectangle(brush_dead, rect.X + 1, rect.Y + 1, rect.Width - 1, rect.Height - 1);
            }
            
            pen.Dispose();
            brush_dead.Dispose();
            brush_alive.Dispose();
        }
    }
    public class EntityMatrix
    {
        int width = 10;
        int height = 10;

        public int Width { get { return width; }
            set
            {
                width = value;
                for (int i = 0; i < size_w; i++)
                {
                    for (int j = 0; j < size_h; j++)
                    {
                        entitys[i, j].rect = new Rectangle(i * width, j * height, width, height);
                    }
                    
                }
            }
        }
        public int Height { get { return height; }
            set
            {
                height = value;
               
                for (int i = 0; i < size_w; i++)
                {
                    for (int j = 0; j < size_h; j++)
                    {
                        entitys[i, j].rect = new Rectangle(i * width, j * height, width, height);
                    }

                }
            }
        }

        int size_w;
        int size_h;
        public int SizeW { get { return size_w; } }
        public int SizeH { get { return size_h; } }
        Entity[,] entitys;
        public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(width);
            sb.Append(",");
            sb.Append(height);
            sb.Append(",");
            sb.Append(size_w);
            sb.Append(",");
            sb.Append(size_h);
            sb.Append(",");
            for (int i = 0; i < size_w; i++)
            {
                for (int j = 0; j < size_h; j++)
                {
                    if(entitys[i,j].is_alive==true) sb.Append(1);
                    else sb.Append(0);
                }
                sb.Append("|");
            }
            return sb.ToString();
        }
        public void LoadString(string str)
        {
            string[] arr = str.Split(new char[] { ',' });
            width = int.Parse(arr[0]);
            height = int.Parse(arr[1]);
            size_w = int.Parse(arr[2]);
            size_h = int.Parse(arr[3]);
            entitys = new Entity[size_w, size_h];

            string data = arr[4];
            arr = data.Split(new char[] { '|' },StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr[i].Length; j++)
                {
                    entitys[i, j] = new Entity();
                    entitys[i, j].x = i;
                    entitys[i, j].y = j;
                    entitys[i, j].rect = new Rectangle(i * width, j * height, width, height);
                    if (arr[i].Substring(j, 1) == "0") entitys[i, j].is_alive = false;
                    else entitys[i, j].is_alive = true; 
                }
            }

        }
        public void InitSize(int SizeW,int SizeH,int width,int height)
        {
            size_w = SizeW;
            size_h = SizeH;
            this.width = width;
            this.height = height;

            entitys = new Entity[size_w, size_h];
            for (int i = 0; i < size_w; i++)
            {
                for (int j = 0; j < size_h; j++)
                {
                    entitys[i, j] = new Entity();
                    entitys[i, j].x = i;
                    entitys[i, j].y = j;
                    entitys[i, j].rect = new Rectangle(i * width, j * height, width, height);


                }
            }
        }
        public void ClearLife()
        {
            for (int i = 0; i < size_w; i++)
            {
                for (int j = 0; j < size_h; j++)
                {
                    entitys[i, j].is_alive = false;
                }
            }
        }
        public void CopyFrom(EntityMatrix em)
        {
            this.width = em.Width;
            this.height = em.height;
            this.size_h = em.SizeH;
            this.size_w = em.SizeW;
            entitys = new Entity[size_w, size_h];
            for (int i = 0; i < size_w; i++)
            {
                for (int j = 0; j < size_h; j++)
                {
                    entitys[i, j] = new Entity();
                    entitys[i, j].x = em.Entitys[i,j].x;
                    entitys[i, j].y = em.Entitys[i,j].y;
                    entitys[i, j].rect = em.Entitys[i, j].rect;
                    entitys[i, j].is_alive = em.Entitys[i, j].is_alive;


                }
            }

        }
        public Entity[,] Entitys { get { return entitys; } }
        public bool ComputeEntityStatus(int x, int y)
        {
            bool status_me = entitys[x, y].is_alive;

            int alive_around_count = 0;
            
            int left_top_x = x - 1; if (left_top_x < 0) left_top_x = size_w - 1;
            int left_top_y = y - 1; if (left_top_y < 0) left_top_y = size_h - 1;
            if (entitys[left_top_x, left_top_y].is_alive == true) alive_around_count++;

            int top_x = x;
            int top_y = y - 1; if (top_y < 0) top_y = size_h - 1;
            if (entitys[top_x, top_y].is_alive == true) alive_around_count++;

            int right_top_x = x + 1; if (right_top_x >= size_w) right_top_x = 0;
            int right_top_y = y - 1; if (right_top_y < 0) right_top_y = size_h - 1;
            if (entitys[right_top_x, right_top_y].is_alive == true) alive_around_count++;

            int left_x = x - 1; if (left_x < 0) left_x = size_w - 1;
            int left_y = y;
            if (entitys[left_x, left_y].is_alive == true) alive_around_count++;

            int right_x = x + 1; if (right_x >= size_w) right_x = 0;
            int right_y = y;
            if (entitys[right_x, right_y].is_alive == true) alive_around_count++;

            int left_bottom_x = x - 1; if (left_bottom_x < 0) left_bottom_x = size_w - 1;
            int left_bottom_y = y + 1; if (left_bottom_y >= size_h) left_bottom_y = 0;
            if (entitys[left_bottom_x, left_bottom_y].is_alive == true) alive_around_count++;

            int bottom_x = x;
            int bottom_y = y + 1; if (bottom_y >= size_h) bottom_y = 0;
            if (entitys[bottom_x, bottom_y].is_alive == true) alive_around_count++;

            int right_bottom_x = x + 1; if (right_bottom_x >= size_w) right_bottom_x = 0;
            int right_bottom_y = y + 1; if (right_bottom_y >= size_h) right_bottom_y = 0;
            if (entitys[right_bottom_x, right_bottom_y].is_alive == true) alive_around_count++;

            if (alive_around_count == 3) status_me = true;
            else if(alive_around_count>3) status_me = false;
            else if(alive_around_count <2) status_me = false;
            
            return status_me;

        }
        public void SetEntityStatus(int x, int y, bool is_alive)
        {
            entitys[x, y].is_alive = is_alive;
        }

        public void Draw(Graphics e)
        {
            for (int i = 0; i < size_w; i++)
            {
                for (int j = 0; j < size_h; j++)
                {
                    entitys[i, j].Draw(e);
                }
            }
        }
        public Entity HitTest(Point point)
        {
            for (int i = 0; i < size_w; i++)
            {
                for (int j = 0; j < size_h; j++)
                {
                    if (entitys[i, j].rect.X <= point.X && entitys[i, j].rect.Y <= point.Y && entitys[i, j].rect.X + width > point.X && entitys[i, j].rect.Y + height > point.Y)
                    {
                        return entitys[i, j];
                    }
                }
            }
            return null;
        }
        
    }
}
