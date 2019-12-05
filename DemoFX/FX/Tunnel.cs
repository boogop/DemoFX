using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    public class Tunnel : Base.BaseGraphics, Base.iDemo
    {
        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;

        public int inDemo = 1;

        colr[] colors;
        starfield starField;
        double pi2 = 6.283184;
       
        public struct star
        {
            public double x, y, z, radius, num_points;
        }
        public struct starfield
        {
            public int num_stars;
            public star[] star_list;
            public double max_z, min_z, z_speed;
        }

        double zPosAdder, angle, amplitude_angle, horizontal_angle, vertical_angle;

        public void init()
        {
            colors = new colr[256];
            standardPalette(ref colors);
            zPosAdder = angle = amplitude_angle = horizontal_angle = vertical_angle = 0;
            starField = create_starfield(20, 200, 36, 2500, 2, 3);
        }


        public void doIt(string msg)
        {
            angle += 0.0001;
            amplitude_angle += .000086;
            horizontal_angle += .0003;
            vertical_angle += 0.000043;

            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();

            process_starfield(starField);
            draw_starfield(starField);

            endIt();
            drawIt();

        }

        private void process_starfield(starfield sfield)
        {
            for (int star_count = 0; star_count < sfield.num_stars; star_count++)
            {
                star s = sfield.star_list[star_count];

                s.z -= sfield.z_speed;
                if (s.z <= sfield.min_z)
                {
                    s.z = sfield.max_z;
                    s.x = (100 + 300 * Math.Cos(amplitude_angle)) * Math.Sin(angle) + 300 * Math.Sin(horizontal_angle);
                }

                s.y = (100 + 300 * Math.Sin(amplitude_angle)) * Math.Cos(angle) + 300 * Math.Sin(vertical_angle);

                sfield.star_list[star_count] = s;
            }

            angle += 0.0001;
            amplitude_angle += .000086;
            horizontal_angle += .0003;
            vertical_angle += 0.000043;
        }

        private void draw_starfield(starfield sfield)
        {
            for (int star_count = 0; star_count < sfield.num_stars; star_count++)
            {
                star s = sfield.star_list[star_count];

                for (double p = 0; p < s.num_points; p++)
                {
                    double x = (theWidth >> 1) + theWidth * (s.x + s.radius * 2 * Math.Sin(p * pi2 / s.num_points)) * 1 / s.z;
                    double y = (theHeight >> 1) - theHeight * (s.y + s.radius * Math.Cos(p * pi2 / s.num_points)) * 1 / s.z;

                    // kludgey color fader lol
                    double cc = 1 - Math.Abs(s.z / (sfield.max_z * .8));
                    if (cc < 0)
                        cc = 0;

                    if (cc > 1)
                        cc = 1;

                    int c = (int)(300 * cc);
                    if (c > 255)
                        c = 255;

                    Color co = Color.FromArgb(colors[c].b, colors[c].r, colors[c].g);
                    if (s.z > (int)(sfield.max_z * .6))
                        co = Color.FromArgb(colors[c].b >> 1, c, c);

                    updateIt((int)x, (int)y, co);
                    updateIt((int)x + 1, (int)y, co);
                    updateIt((int)x, (int)y + 1, co);
                }
            }
        }

        private starfield create_starfield(int num_stars, double radius, double num_points, double max_z, double min_z, double z_speed)
        {
            zPosAdder = max_z / num_stars;
            double zPos = max_z;

            starfield s = new starfield();
            s.star_list = new star[num_stars];
            for (int i = 0; i < num_stars; i++)
            {
                s.star_list[i].radius = radius;
                s.star_list[i].num_points = num_points;
                s.star_list[i].z = min_z + ((max_z - min_z) / num_stars) * i + 1;
                zPos -= zPosAdder;
            }
            s.max_z = max_z;
            s.min_z = min_z;
            s.z_speed = z_speed;
            s.num_stars = num_stars;
            return s;
        }

    }
}
