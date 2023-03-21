/*
	TUIO C# Demo - part of the reacTIVision project
	Copyright (c) 2005-2016 Martin Kaltenbrunner <martin@tuio.org>

	This program is free software; you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using TUIO;
using AxWMPLib;
using TuioDemo1;
using System.IO;

public class TuioDemo : Form , TuioListener
	{
		
		private TuioClient client;
		private Dictionary<long,TuioObject> objectList;
		private Dictionary<long,TuioCursor> cursorList;
		private Dictionary<long,TuioBlob> blobList;

		public static int width, height;
		private int window_width =  1040;
		private int window_height = 1080;
		private int window_left = 0;
		private int window_top = 0;
		private int screen_width = Screen.PrimaryScreen.Bounds.Width;
		private int screen_height = Screen.PrimaryScreen.Bounds.Height;

		private bool fullscreen;
		private bool verbose;

		Font font = new Font("Arial", 10.0f);
		SolidBrush fntBrush = new SolidBrush(Color.White);
		SolidBrush bgrBrush = new SolidBrush(Color.White);
		SolidBrush curBrush = new SolidBrush(Color.FromArgb(192, 0, 192));
		SolidBrush objBrush = new SolidBrush(Color.Green);
		SolidBrush blbBrush = new SolidBrush(Color.FromArgb(64, 64, 64));
		Pen curPen = new Pen(new SolidBrush(Color.Blue), 1);
    private AxWindowsMediaPlayer axWindowsMediaPlayer1;
    Bitmap Body = new Bitmap("11.jpg");

    public TuioDemo(int port) {

		
		InitializeComponent();
        verbose = false;
			fullscreen = false;
			width = window_width;
			height = window_height;
		

        this.ClientSize = new System.Drawing.Size(width, height);
			this.Name = "TuioDemo";
			this.Text = "TuioDemo";
			
			this.Closing+=new CancelEventHandler(Form_Closing);
			this.KeyDown+=new KeyEventHandler(Form_KeyDown);
        

        this.SetStyle( ControlStyles.AllPaintingInWmPaint |
							ControlStyles.UserPaint |
							ControlStyles.DoubleBuffer, true);

			objectList = new Dictionary<long,TuioObject>(128);
			cursorList = new Dictionary<long,TuioCursor>(128);
			blobList   = new Dictionary<long,TuioBlob>(128);
			
			client = new TuioClient(port);
			client.addTuioListener(this);

			client.connect();
		}

		private void Form_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {

 			if ( e.KeyData == Keys.F1) {
	 			if (fullscreen == false) {

					width = screen_width;
					height = screen_height;

					window_left = this.Left;
					window_top = this.Top;

					this.FormBorderStyle = FormBorderStyle.None;
		 			this.Left = 0;
		 			this.Top = 0;
		 			this.Width = screen_width;
		 			this.Height = screen_height;

		 			fullscreen = true;
	 			} else {

					width = window_width;
					height = window_height;

		 			this.FormBorderStyle = FormBorderStyle.Sizable;
		 			this.Left = window_left;
		 			this.Top = window_top;
		 			this.Width = window_width;
		 			this.Height = window_height;

		 			fullscreen = false;
	 			}
 			} else if ( e.KeyData == Keys.Escape) {
				this.Close();

 			} else if ( e.KeyData == Keys.V ) {
 				verbose=!verbose;
 			}

 		}

		private void Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			client.removeTuioListener(this);

			client.disconnect();
			System.Environment.Exit(0);
		}
   
    public void openvideoform(TuioObject o) 
	{
        Video video = new Video(o.video);
        video.Show();
    }
    public void addTuioObject(TuioObject o) {
			lock(objectList) {
   //         string v = o.SymbolID.ToString() + ".mp4";
   //         string path = "C:\\Users\\abdullah\\Desktop\\" + v;
			//o.video = path;
			//openvideoform(o);
            objectList.Add(o.SessionID,o);
			
				
           
        } if (verbose) Console.WriteLine("add obj "+o.SymbolID+ " (" + o.SessionID+") "+o.X+" "+o.Y+" "+o.Angle);
    }

		public void updateTuioObject(TuioObject o) {

			if (verbose) Console.WriteLine("set obj "+o.SymbolID+" "+o.SessionID+" "+o.X+" "+o.Y+" "+o.Angle+" "+o.MotionSpeed+" "+o.RotationSpeed+" "+o.MotionAccel+" "+o.RotationAccel);
		}

		public void removeTuioObject(TuioObject o) {
			lock(objectList) {
				objectList.Remove(o.SessionID);
			}
			if (verbose) Console.WriteLine("del obj "+o.SymbolID+" ("+o.SessionID+")");
		}

		public void addTuioCursor(TuioCursor c) {
			lock(cursorList) {
				cursorList.Add(c.SessionID,c);
			}
			if (verbose) Console.WriteLine("add cur "+c.CursorID + " ("+c.SessionID+") "+c.X+" "+c.Y);
		}

		public void updateTuioCursor(TuioCursor c) {
			if (verbose) Console.WriteLine("set cur "+c.CursorID + " ("+c.SessionID+") "+c.X+" "+c.Y+" "+c.MotionSpeed+" "+c.MotionAccel);
		}

		public void removeTuioCursor(TuioCursor c) {
			lock(cursorList) {
				cursorList.Remove(c.SessionID);
			}
			if (verbose) Console.WriteLine("del cur "+c.CursorID + " ("+c.SessionID+")");
 		}

		public void addTuioBlob(TuioBlob b) {
			lock(blobList) {
				blobList.Add(b.SessionID,b);
			}
			if (verbose) Console.WriteLine("add blb "+b.BlobID + " ("+b.SessionID+") "+b.X+" "+b.Y+" "+b.Angle+" "+b.Width+" "+b.Height+" "+b.Area);
		}

		public void updateTuioBlob(TuioBlob b) {
		
			if (verbose) Console.WriteLine("set blb "+b.BlobID + " ("+b.SessionID+") "+b.X+" "+b.Y+" "+b.Angle+" "+b.Width+" "+b.Height+" "+b.Area+" "+b.MotionSpeed+" "+b.RotationSpeed+" "+b.MotionAccel+" "+b.RotationAccel);
		}

		public void removeTuioBlob(TuioBlob b) {
			lock(blobList) {
				blobList.Remove(b.SessionID);
			}
			if (verbose) Console.WriteLine("del blb "+b.BlobID + " ("+b.SessionID+")");
		}

		public void refresh(TuioTime frameTime) {
			Invalidate();
		}
    /// <summary>
    /// OnPaintBackground //mohem draw simpol
    /// paint  //mohem
    /// </summary>
    /// <param name="pevent"></param>
	public int Count=0;
    protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			// Getting the graphics object
			Graphics g = pevent.Graphics;
			g.FillRectangle(bgrBrush, new Rectangle(0,0,width,height));
			g.DrawImage(Body, 50, 50, 200, 400);

			// draw the cursor path
			if (cursorList.Count > 0) {
 			 lock(cursorList) {
			 foreach (TuioCursor tcur in cursorList.Values) {
					List<TuioPoint> path = tcur.Path;
					TuioPoint current_point = path[0];

					for (int i = 0; i < path.Count; i++) {
						TuioPoint next_point = path[i];
						g.DrawLine(curPen, current_point.getScreenX(width), current_point.getScreenY(height), next_point.getScreenX(width), next_point.getScreenY(height));
						current_point = next_point;
					}
					g.FillEllipse(curBrush, current_point.getScreenX(width) - height / 100, current_point.getScreenY(height) - height / 100, height / 50, height / 50);
					g.DrawString(tcur.CursorID + "", font, fntBrush, new PointF(tcur.getScreenX(width) - 10, tcur.getScreenY(height) - 10));
				}
			}
		 }

			// draw the objects
			if (objectList.Count > 0) {
 				lock(objectList) {
					foreach (TuioObject tobj in objectList.Values) {
						int ox = tobj.getScreenX(width);
						int oy = tobj.getScreenY(height);
						int size = height / 10;

						g.TranslateTransform(ox, oy);
						g.RotateTransform((float)(tobj.Angle / Math.PI * 180.0f));
						g.TranslateTransform(-ox, -oy);

						

						g.TranslateTransform(ox, oy);
						g.RotateTransform(-1 * (float)(tobj.Angle / Math.PI * 180.0f));
						g.TranslateTransform(-ox, -oy);

						//g.FillRectangle(objBrush, new Rectangle(ox - size / 2, oy - size / 2, size, size));
						//g.DrawString(tobj.SymbolID + "", font, fntBrush, new PointF(ox, oy ));

					    g.FillEllipse(Brushes.Red, ox, oy, 15, 15);

						// ßÊÝ
						if (ox >= 110 && ox <= (110 + 15) && oy >= 110 && oy <= (110 + 15))
						{
							axWindowsMediaPlayer1.URL = "C:\\Users\\abdullah\\Desktop\\sho.mp4";

						}
						else if (ox >= 170 && ox <= (170 + 15) && oy >= 110 && oy <= (110 + 15))
						{
							axWindowsMediaPlayer1.URL = "C:\\Users\\abdullah\\Desktop\\sho.mp4";
						}
						//ÈØä 
						else if (ox >= 142 && ox <= (142 + 15) && oy >= 170 && oy <= (170 + 15)) 
						{
							axWindowsMediaPlayer1.URL = "C:\\Users\\abdullah\\Desktop\\but.mp4";
						}
						else if (ox >= 142 && ox <= (142 + 15) && oy >= 190 && oy <= (190 + 15)) 
						{
							axWindowsMediaPlayer1.URL = "C:\\Users\\abdullah\\Desktop\\but.mp4";
						}
						// ÑÌá
						else if (ox >= 120 && ox <= (120 + 15) && oy >= 260 && oy <= (260 + 15))
						{
							
							
							axWindowsMediaPlayer1.URL = "C:\\Users\\abdullah\\Desktop\\2.mp4";
						}
						else if (ox >= 160 && ox <= (160 + 15) && oy >= 260 && oy <= (260 + 15))
						{
							axWindowsMediaPlayer1.URL = "C:\\Users\\abdullah\\Desktop\\2.mp4";
						}
						// ÝÎÏ
						else if (ox >= 120 && ox <= (120 + 15) && oy >= 360 && oy <= (360 + 15))
						{
							axWindowsMediaPlayer1.URL = "C:\\Users\\abdullah\\Desktop\\leg.mp4";
						}
						else if (ox >= 165 && ox <= (165 + 15) && oy >= 360 && oy <= (360 + 15))
						{
							axWindowsMediaPlayer1.URL = "C:\\Users\\abdullah\\Desktop\\leg.mp4";
						}


                }
            }
			}

				
				// ßÊÝ
				g.FillEllipse(Brushes.Blue, 110, 110, 15, 15);
				g.FillEllipse(Brushes.Blue, 170, 110, 15, 15);
				g.DrawString("1", font, Brushes.White, new PointF(110,110));
				g.DrawString("1", font, Brushes.White, new PointF(170,110));
				//ÈØä 
				g.FillEllipse(Brushes.Gray, 142, 170, 15, 15);
				g.FillEllipse(Brushes.Gray, 142, 190, 15, 15);
				g.DrawString("2", font, Brushes.White, new PointF(142, 170));
				g.DrawString("2", font, Brushes.White, new PointF(142, 190));
				// ÑÌá 
				g.FillEllipse(Brushes.Orange, 120, 260, 15, 15);
				g.FillEllipse(Brushes.Orange, 160, 260, 15, 15);
				g.DrawString("3", font, Brushes.White, new PointF(120, 260));
				g.DrawString("3", font, Brushes.White, new PointF(160, 260));
				//ÝÎÐ 
				g.FillEllipse(Brushes.Green, 120, 360, 15, 15);
				g.FillEllipse(Brushes.Green, 165, 360, 15, 15);
				g.DrawString("4", font, Brushes.White, new PointF(120, 360));
				g.DrawString("4", font, Brushes.White, new PointF(165, 360));

        /*
     //g.FillRectangle(objBrush, new Rectangle(ox - size / 2, oy - size / 2, size, size));
     //g.DrawString(tobj.SymbolID + "", font, fntBrush, new PointF(ox - 10, oy - 10));
     //g.DrawString(ox.ToString()+" "+oy.ToString() + "", font, fntBrush, new PointF(ox - 10, oy - 10));
     //g.DrawImage(m,new PointF(ox - 10, oy - 10),ox,oy);



     //if (ox >= 500 && oy >= 200)
     //{
     //    MessageBox.Show("ggg");

     //video = new Video(tobj.video);
     //video.ShowDialog();
     //if (tobj.SymbolID == 0 &&      Count==0) { video = new Video("C:\\Users\\abdullah\\Desktop\\1.mp4"); video.Show(); Count++; }
     //else if (tobj.SymbolID == 1 && Count==0) { video = new Video("C:\\Users\\abdullah\\Desktop\\2.mp4"); video.Show(); Count++; }
     //else if (tobj.SymbolID == 2 && Count==0) { video = new Video("C:\\Users\\abdullah\\Desktop\\3.mov"); video.Show(); Count++; }
     //else if (tobj.SymbolID == 3 && Count==0) { video = new Video("C:\\Users\\abdullah\\Desktop\\4.mp4"); video.Show(); Count++; }
     //else if (tobj.SymbolID == 4 && Count==0) { video = new Video("C:\\Users\\abdullah\\Desktop\\5.mp4"); video.Show(); Count++; }
     //else if (tobj.SymbolID == 5 && Count==0) { video = new Video("C:\\Users\\abdullah\\Desktop\\6.mp4"); video.Show(); Count++; }
     //else if (tobj.SymbolID > 5  && Count==0) {MessageBox.Show("null"); Count ++;}


     //}

     //for (int i=0;i<objectList.Count;i++) 
     //{
     //	//objectList[i].SymbolID.ToString();
     //	if (objectList[i].SymbolID == 0) { video = new Video("C:\\Users\\abdullah\\Desktop\\1.mp4"); video.Show(); }
     //	else if (objectList[i].SymbolID == 1) { MessageBox.Show("C:\\Users\\abdullah\\Desktop\\2.mp4"); }
     //	else if (objectList[i].SymbolID == 2) { MessageBox.Show("C:\\Users\\abdullah\\Desktop\\3.mp4"); }
     //	else if (objectList[i].SymbolID == 3) { MessageBox.Show("C:\\Users\\abdullah\\Desktop\\4.mp4"); }
     //	else if (objectList[i].SymbolID == 4) { MessageBox.Show("C:\\Users\\abdullah\\Desktop\\5.mp4"); }
     //	else if (objectList[i].SymbolID == 5) { MessageBox.Show("C:\\Users\\abdullah\\Desktop\\6.mp4"); }
     //	else 
     //	{
     //              MessageBox.Show("null");
     //          }

     //      }
     //MediaPlayerTraining.URL = "C:\\Users\\abdullah\\Desktop\\2.mp4";


     //// Play the video.
     //MediaPlayerTraining.Ctlcontrols.play();
     //g.DrawEllipse(Pens.Green,500,200,80,80);
     */

        // draw the blobs
        if (blobList.Count > 0) {
				lock(blobList) {
					foreach (TuioBlob tblb in blobList.Values) {
						int bx = tblb.getScreenX(width);
						int by = tblb.getScreenY(height);
						float bw = tblb.Width*width;
						float bh = tblb.Height*height;

						g.TranslateTransform(bx, by);
						g.RotateTransform((float)(tblb.Angle / Math.PI * 180.0f));
						g.TranslateTransform(-bx, -by);

						g.FillEllipse(blbBrush, bx - bw / 2, by - bh / 2, bw, bh);

						g.TranslateTransform(bx, by);
						g.RotateTransform(-1 * (float)(tblb.Angle / Math.PI * 180.0f));
						g.TranslateTransform(-bx, -by);
						
						g.DrawString(tblb.BlobID + "", font, fntBrush, new PointF(bx, by));
					}
				}
			}
		}
	/// <summary>
	/// this my work  
	/// </summary>
    private void InitializeComponent()
    {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TuioDemo));
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.SuspendLayout();
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(653, 31);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(378, 362);
            this.axWindowsMediaPlayer1.TabIndex = 0;
            // 
            // TuioDemo
            // 
            this.ClientSize = new System.Drawing.Size(920, 762);
            this.Controls.Add(this.axWindowsMediaPlayer1);
            this.Name = "TuioDemo";
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            this.ResumeLayout(false);

    }

    [STAThread]
    public static void Main(String[] argv) {
	 		int port = 0;
			switch (argv.Length) {
				case 1:
					port = int.Parse(argv[0],null);
					if(port==0) goto default;
					break;
				case 0:
					port = 3333;
					break;
				default:
					Console.WriteLine("usage: mono TuioDemo [port]");
					System.Environment.Exit(0);
					break;
			}
			
			TuioDemo app = new TuioDemo(port);		
			Application.Run(app);
		}
	}
