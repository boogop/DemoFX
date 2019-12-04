using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

#pragma warning disable 0649


namespace DemoFX
{
    public partial class Form1 : Form
    {
        private int _marginWidth = 25;
        private int _marginHeight = 25;
        private Bitmap bmp;
        private Graphics gForm;
        private Graphics gBmp;
        private int fHeight, fWidth;
        private int midWidth, midHeight;
        private Random rand;
        private FastBitmap fb;
        const double PI = 3.14159;
        Base.DrawScreen cDraw;
        bool doit = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            List<string> a = new List<string>();
            var type = typeof(Base.iDemo);

            foreach (Type t in asm.GetTypes())
            {
                if(type.IsAssignableFrom(t) && t.Name.ToUpper() != "IDEMO")
                    a.Add(t.Name);               
            }

            a.Sort();

            for (int i = 0; i < a.Count; i++)
                listBox1.Items.Add(a[i]);

            init();

        }


        private void init()
        {
            bmp = new Bitmap(pBox1.Width, pBox1.Height);
            gForm = pBox1.CreateGraphics();
            gBmp = Graphics.FromImage(bmp);
            rand = new Random();
            fHeight = bmp.Height;
            fWidth = bmp.Width;
            midWidth = fWidth / 2;
            midHeight = fHeight / 2;

            cDraw = new Base.DrawScreen();

            initForm(cDraw);
            cDraw.theWidth = fWidth;
            cDraw.theHeight = fHeight;
            cDraw.theMarginWidth = cDraw.theMarginHeight = 15;
        }

        private void initialize(ref Base.BaseGraphics g)
        {
            g.theBMP = bmp;
            g.theFB = fb;
            g.theGBMP = gBmp;
            g.theGForm = gForm;
            g.theWidth = fWidth;
            g.theHeight = fHeight;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = listBox1.Text;
            doit = true;
            boom(s);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            doit = false;
        }

        private void initForm<T>(T obj) where T : Base.BaseGraphics, new()
        {
            obj.theBMP = bmp;
            obj.theFB = fb;
            obj.theGBMP = gBmp;
            obj.theGForm = gForm;
            obj.theWidth = fWidth;
            obj.theHeight = fHeight;
            obj.theMarginHeight = _marginHeight;
            obj.theMarginWidth = _marginWidth;

        }

        private void boom(string classname)
        {
            var type = Type.GetType("DemoFX.FX." + classname);
            var obj = (Base.BaseGraphics)Activator.CreateInstance(type);

            obj.theBMP = bmp;
            obj.theFB = fb;
            obj.theGBMP = gBmp;
            obj.theGForm = gForm;
            obj.theWidth = fWidth;
            obj.theHeight = fHeight;
            obj.theMarginHeight = _marginHeight;
            obj.theMarginWidth = _marginWidth;

            MethodInfo methodInfo = type.GetMethod("init");
            methodInfo.Invoke(obj, null);

            methodInfo = type.GetMethod("doIt");
            string[] t = new string[1];
            t[0] = classname;

            while (doit)
            {
                Application.DoEvents();
                methodInfo.Invoke(obj, t);
            }

        }

    }
}
