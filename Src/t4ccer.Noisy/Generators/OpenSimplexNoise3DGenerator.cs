﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace t4ccer.Noisy
{
    public class OpenSimplexNoise3DGenerator : INoise
    {
        
        
        
        
        private static readonly int[] gradients3D = new int[] {
        -11,  4,  4,     -4,  11,  4,    -4,  4,  11,
         11,  4,  4,      4,  11,  4,     4,  4,  11,
        -11, -4,  4,     -4, -11,  4,    -4, -4,  11,
         11, -4,  4,      4, -11,  4,     4, -4,  11,
        -11,  4, -4,     -4,  11, -4,    -4,  4, -11,
         11,  4, -4,      4,  11, -4,     4,  4, -11,
        -11, -4, -4,     -4, -11, -4,    -4, -4, -11,
         11, -4, -4,      4, -11, -4,     4, -4, -11,
        };

        private const double STRETCH_CONSTANT_3D = -1.0 / 6;
        private const double SQUISH_CONSTANT_3D = 1.0 / 3;

        private const double NORM_CONSTANT_3D = 103;

        private short[] perm;
        private short[] permGradIndex3D;

        public OpenSimplexNoise3DGenerator() : this(new Random().Next()) { }
        public OpenSimplexNoise3DGenerator(short[] perm)
        {
            this.perm = perm;
            permGradIndex3D = new short[256];

            for (int i = 0; i < 256; i++)
            {
                
                permGradIndex3D[i] = (short)((perm[i] % (gradients3D.Length / 3)) * 3);
            }
        }

        
        
        
        public OpenSimplexNoise3DGenerator(long seed)
        {
            perm = new short[256];
            permGradIndex3D = new short[256];
            short[] source = new short[256];
            for (short i = 0; i < 256; i++)
                source[i] = i;
            seed = seed * 6364136223846793005L + 1442695040888963407L;
            seed = seed * 6364136223846793005L + 1442695040888963407L;
            seed = seed * 6364136223846793005L + 1442695040888963407L;
            for (int i = 255; i >= 0; i--)
            {
                seed = seed * 6364136223846793005L + 1442695040888963407L;
                int r = (int)((seed + 31) % (i + 1));
                if (r < 0)
                    r += (i + 1);
                perm[i] = source[r];
                permGradIndex3D[i] = (short)((perm[i] % (gradients3D.Length / 3)) * 3);
                source[r] = source[i];
            }
        }

        public double At(double x, double y)
            => At(x, y, 0);
		public double At(double x, double y, double z)
		{

			
			double stretchOffset = (x + y + z) * STRETCH_CONSTANT_3D;
			double xs = x + stretchOffset;
			double ys = y + stretchOffset;
			double zs = z + stretchOffset;

			
			int xsb = FastFloor(xs);
			int ysb = FastFloor(ys);
			int zsb = FastFloor(zs);

			
			double squishOffset = (xsb + ysb + zsb) * SQUISH_CONSTANT_3D;
			double xb = xsb + squishOffset;
			double yb = ysb + squishOffset;
			double zb = zsb + squishOffset;

			
			double xins = xs - xsb;
			double yins = ys - ysb;
			double zins = zs - zsb;

			
			double inSum = xins + yins + zins;

			
			double dx0 = x - xb;
			double dy0 = y - yb;
			double dz0 = z - zb;

			
			double dx_ext0, dy_ext0, dz_ext0;
			double dx_ext1, dy_ext1, dz_ext1;
			int xsv_ext0, ysv_ext0, zsv_ext0;
			int xsv_ext1, ysv_ext1, zsv_ext1;

			double value = 0;
			if (inSum <= 1)
			{ 

				
				byte aPoint = 0x01;
				double aScore = xins;
				byte bPoint = 0x02;
				double bScore = yins;
				if (aScore >= bScore && zins > bScore)
				{
					bScore = zins;
					bPoint = 0x04;
				}
				else if (aScore < bScore && zins > aScore)
				{
					aScore = zins;
					aPoint = 0x04;
				}

				
				
				double wins = 1 - inSum;
				if (wins > aScore || wins > bScore)
				{ 
					byte c = (bScore > aScore ? bPoint : aPoint); 

					if ((c & 0x01) == 0)
					{
						xsv_ext0 = xsb - 1;
						xsv_ext1 = xsb;
						dx_ext0 = dx0 + 1;
						dx_ext1 = dx0;
					}
					else
					{
						xsv_ext0 = xsv_ext1 = xsb + 1;
						dx_ext0 = dx_ext1 = dx0 - 1;
					}

					if ((c & 0x02) == 0)
					{
						ysv_ext0 = ysv_ext1 = ysb;
						dy_ext0 = dy_ext1 = dy0;
						if ((c & 0x01) == 0)
						{
							ysv_ext1 -= 1;
							dy_ext1 += 1;
						}
						else
						{
							ysv_ext0 -= 1;
							dy_ext0 += 1;
						}
					}
					else
					{
						ysv_ext0 = ysv_ext1 = ysb + 1;
						dy_ext0 = dy_ext1 = dy0 - 1;
					}

					if ((c & 0x04) == 0)
					{
						zsv_ext0 = zsb;
						zsv_ext1 = zsb - 1;
						dz_ext0 = dz0;
						dz_ext1 = dz0 + 1;
					}
					else
					{
						zsv_ext0 = zsv_ext1 = zsb + 1;
						dz_ext0 = dz_ext1 = dz0 - 1;
					}
				}
				else
				{ 
					byte c = (byte)(aPoint | bPoint); 

					if ((c & 0x01) == 0)
					{
						xsv_ext0 = xsb;
						xsv_ext1 = xsb - 1;
						dx_ext0 = dx0 - 2 * SQUISH_CONSTANT_3D;
						dx_ext1 = dx0 + 1 - SQUISH_CONSTANT_3D;
					}
					else
					{
						xsv_ext0 = xsv_ext1 = xsb + 1;
						dx_ext0 = dx0 - 1 - 2 * SQUISH_CONSTANT_3D;
						dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_3D;
					}

					if ((c & 0x02) == 0)
					{
						ysv_ext0 = ysb;
						ysv_ext1 = ysb - 1;
						dy_ext0 = dy0 - 2 * SQUISH_CONSTANT_3D;
						dy_ext1 = dy0 + 1 - SQUISH_CONSTANT_3D;
					}
					else
					{
						ysv_ext0 = ysv_ext1 = ysb + 1;
						dy_ext0 = dy0 - 1 - 2 * SQUISH_CONSTANT_3D;
						dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_3D;
					}

					if ((c & 0x04) == 0)
					{
						zsv_ext0 = zsb;
						zsv_ext1 = zsb - 1;
						dz_ext0 = dz0 - 2 * SQUISH_CONSTANT_3D;
						dz_ext1 = dz0 + 1 - SQUISH_CONSTANT_3D;
					}
					else
					{
						zsv_ext0 = zsv_ext1 = zsb + 1;
						dz_ext0 = dz0 - 1 - 2 * SQUISH_CONSTANT_3D;
						dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_3D;
					}
				}

				
				double attn0 = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0;
				if (attn0 > 0)
				{
					attn0 *= attn0;
					value += attn0 * attn0 * Extrapolate(xsb + 0, ysb + 0, zsb + 0, dx0, dy0, dz0);
				}

				
				double dx1 = dx0 - 1 - SQUISH_CONSTANT_3D;
				double dy1 = dy0 - 0 - SQUISH_CONSTANT_3D;
				double dz1 = dz0 - 0 - SQUISH_CONSTANT_3D;
				double attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1;
				if (attn1 > 0)
				{
					attn1 *= attn1;
					value += attn1 * attn1 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, dx1, dy1, dz1);
				}

				
				double dx2 = dx0 - 0 - SQUISH_CONSTANT_3D;
				double dy2 = dy0 - 1 - SQUISH_CONSTANT_3D;
				double dz2 = dz1;
				double attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2;
				if (attn2 > 0)
				{
					attn2 *= attn2;
					value += attn2 * attn2 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, dx2, dy2, dz2);
				}

				
				double dx3 = dx2;
				double dy3 = dy1;
				double dz3 = dz0 - 1 - SQUISH_CONSTANT_3D;
				double attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3;
				if (attn3 > 0)
				{
					attn3 *= attn3;
					value += attn3 * attn3 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, dx3, dy3, dz3);
				}
			}
			else if (inSum >= 2)
			{ 

				
				byte aPoint = 0x06;
				double aScore = xins;
				byte bPoint = 0x05;
				double bScore = yins;
				if (aScore <= bScore && zins < bScore)
				{
					bScore = zins;
					bPoint = 0x03;
				}
				else if (aScore > bScore && zins < aScore)
				{
					aScore = zins;
					aPoint = 0x03;
				}

				
				
				double wins = 3 - inSum;
				if (wins < aScore || wins < bScore)
				{ 
					byte c = (bScore < aScore ? bPoint : aPoint); 

					if ((c & 0x01) != 0)
					{
						xsv_ext0 = xsb + 2;
						xsv_ext1 = xsb + 1;
						dx_ext0 = dx0 - 2 - 3 * SQUISH_CONSTANT_3D;
						dx_ext1 = dx0 - 1 - 3 * SQUISH_CONSTANT_3D;
					}
					else
					{
						xsv_ext0 = xsv_ext1 = xsb;
						dx_ext0 = dx_ext1 = dx0 - 3 * SQUISH_CONSTANT_3D;
					}

					if ((c & 0x02) != 0)
					{
						ysv_ext0 = ysv_ext1 = ysb + 1;
						dy_ext0 = dy_ext1 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D;
						if ((c & 0x01) != 0)
						{
							ysv_ext1 += 1;
							dy_ext1 -= 1;
						}
						else
						{
							ysv_ext0 += 1;
							dy_ext0 -= 1;
						}
					}
					else
					{
						ysv_ext0 = ysv_ext1 = ysb;
						dy_ext0 = dy_ext1 = dy0 - 3 * SQUISH_CONSTANT_3D;
					}

					if ((c & 0x04) != 0)
					{
						zsv_ext0 = zsb + 1;
						zsv_ext1 = zsb + 2;
						dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_3D;
						dz_ext1 = dz0 - 2 - 3 * SQUISH_CONSTANT_3D;
					}
					else
					{
						zsv_ext0 = zsv_ext1 = zsb;
						dz_ext0 = dz_ext1 = dz0 - 3 * SQUISH_CONSTANT_3D;
					}
				}
				else
				{ 
					byte c = (byte)(aPoint & bPoint); 

					if ((c & 0x01) != 0)
					{
						xsv_ext0 = xsb + 1;
						xsv_ext1 = xsb + 2;
						dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_3D;
						dx_ext1 = dx0 - 2 - 2 * SQUISH_CONSTANT_3D;
					}
					else
					{
						xsv_ext0 = xsv_ext1 = xsb;
						dx_ext0 = dx0 - SQUISH_CONSTANT_3D;
						dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D;
					}

					if ((c & 0x02) != 0)
					{
						ysv_ext0 = ysb + 1;
						ysv_ext1 = ysb + 2;
						dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_3D;
						dy_ext1 = dy0 - 2 - 2 * SQUISH_CONSTANT_3D;
					}
					else
					{
						ysv_ext0 = ysv_ext1 = ysb;
						dy_ext0 = dy0 - SQUISH_CONSTANT_3D;
						dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D;
					}

					if ((c & 0x04) != 0)
					{
						zsv_ext0 = zsb + 1;
						zsv_ext1 = zsb + 2;
						dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_3D;
						dz_ext1 = dz0 - 2 - 2 * SQUISH_CONSTANT_3D;
					}
					else
					{
						zsv_ext0 = zsv_ext1 = zsb;
						dz_ext0 = dz0 - SQUISH_CONSTANT_3D;
						dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D;
					}
				}

				
				double dx3 = dx0 - 1 - 2 * SQUISH_CONSTANT_3D;
				double dy3 = dy0 - 1 - 2 * SQUISH_CONSTANT_3D;
				double dz3 = dz0 - 0 - 2 * SQUISH_CONSTANT_3D;
				double attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3;
				if (attn3 > 0)
				{
					attn3 *= attn3;
					value += attn3 * attn3 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, dx3, dy3, dz3);
				}

				
				double dx2 = dx3;
				double dy2 = dy0 - 0 - 2 * SQUISH_CONSTANT_3D;
				double dz2 = dz0 - 1 - 2 * SQUISH_CONSTANT_3D;
				double attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2;
				if (attn2 > 0)
				{
					attn2 *= attn2;
					value += attn2 * attn2 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, dx2, dy2, dz2);
				}

				
				double dx1 = dx0 - 0 - 2 * SQUISH_CONSTANT_3D;
				double dy1 = dy3;
				double dz1 = dz2;
				double attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1;
				if (attn1 > 0)
				{
					attn1 *= attn1;
					value += attn1 * attn1 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, dx1, dy1, dz1);
				}

				
				dx0 = dx0 - 1 - 3 * SQUISH_CONSTANT_3D;
				dy0 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D;
				dz0 = dz0 - 1 - 3 * SQUISH_CONSTANT_3D;
				double attn0 = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0;
				if (attn0 > 0)
				{
					attn0 *= attn0;
					value += attn0 * attn0 * Extrapolate(xsb + 1, ysb + 1, zsb + 1, dx0, dy0, dz0);
				}
			}
			else
			{ 
				double aScore;
				byte aPoint;
				bool aIsFurtherSide;
				double bScore;
				byte bPoint;
				bool bIsFurtherSide;

				
				double p1 = xins + yins;
				if (p1 > 1)
				{
					aScore = p1 - 1;
					aPoint = 0x03;
					aIsFurtherSide = true;
				}
				else
				{
					aScore = 1 - p1;
					aPoint = 0x04;
					aIsFurtherSide = false;
				}

				
				double p2 = xins + zins;
				if (p2 > 1)
				{
					bScore = p2 - 1;
					bPoint = 0x05;
					bIsFurtherSide = true;
				}
				else
				{
					bScore = 1 - p2;
					bPoint = 0x02;
					bIsFurtherSide = false;
				}

				
				double p3 = yins + zins;
				if (p3 > 1)
				{
					double score = p3 - 1;
					if (aScore <= bScore && aScore < score)
					{
						aScore = score;
						aPoint = 0x06;
						aIsFurtherSide = true;
					}
					else if (aScore > bScore && bScore < score)
					{
						bScore = score;
						bPoint = 0x06;
						bIsFurtherSide = true;
					}
				}
				else
				{
					double score = 1 - p3;
					if (aScore <= bScore && aScore < score)
					{
						aScore = score;
						aPoint = 0x01;
						aIsFurtherSide = false;
					}
					else if (aScore > bScore && bScore < score)
					{
						bScore = score;
						bPoint = 0x01;
						bIsFurtherSide = false;
					}
				}

				
				if (aIsFurtherSide == bIsFurtherSide)
				{
					if (aIsFurtherSide)
					{ 

						
						dx_ext0 = dx0 - 1 - 3 * SQUISH_CONSTANT_3D;
						dy_ext0 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D;
						dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_3D;
						xsv_ext0 = xsb + 1;
						ysv_ext0 = ysb + 1;
						zsv_ext0 = zsb + 1;

						
						byte c = (byte)(aPoint & bPoint);
						if ((c & 0x01) != 0)
						{
							dx_ext1 = dx0 - 2 - 2 * SQUISH_CONSTANT_3D;
							dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D;
							dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D;
							xsv_ext1 = xsb + 2;
							ysv_ext1 = ysb;
							zsv_ext1 = zsb;
						}
						else if ((c & 0x02) != 0)
						{
							dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D;
							dy_ext1 = dy0 - 2 - 2 * SQUISH_CONSTANT_3D;
							dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D;
							xsv_ext1 = xsb;
							ysv_ext1 = ysb + 2;
							zsv_ext1 = zsb;
						}
						else
						{
							dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D;
							dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D;
							dz_ext1 = dz0 - 2 - 2 * SQUISH_CONSTANT_3D;
							xsv_ext1 = xsb;
							ysv_ext1 = ysb;
							zsv_ext1 = zsb + 2;
						}
					}
					else
					{

						
						dx_ext0 = dx0;
						dy_ext0 = dy0;
						dz_ext0 = dz0;
						xsv_ext0 = xsb;
						ysv_ext0 = ysb;
						zsv_ext0 = zsb;

						
						byte c = (byte)(aPoint | bPoint);
						if ((c & 0x01) == 0)
						{
							dx_ext1 = dx0 + 1 - SQUISH_CONSTANT_3D;
							dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_3D;
							dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_3D;
							xsv_ext1 = xsb - 1;
							ysv_ext1 = ysb + 1;
							zsv_ext1 = zsb + 1;
						}
						else if ((c & 0x02) == 0)
						{
							dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_3D;
							dy_ext1 = dy0 + 1 - SQUISH_CONSTANT_3D;
							dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_3D;
							xsv_ext1 = xsb + 1;
							ysv_ext1 = ysb - 1;
							zsv_ext1 = zsb + 1;
						}
						else
						{
							dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_3D;
							dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_3D;
							dz_ext1 = dz0 + 1 - SQUISH_CONSTANT_3D;
							xsv_ext1 = xsb + 1;
							ysv_ext1 = ysb + 1;
							zsv_ext1 = zsb - 1;
						}
					}
				}
				else
				{ 
					byte c1, c2;
					if (aIsFurtherSide)
					{
						c1 = aPoint;
						c2 = bPoint;
					}
					else
					{
						c1 = bPoint;
						c2 = aPoint;
					}

					
					if ((c1 & 0x01) == 0)
					{
						dx_ext0 = dx0 + 1 - SQUISH_CONSTANT_3D;
						dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_3D;
						dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_3D;
						xsv_ext0 = xsb - 1;
						ysv_ext0 = ysb + 1;
						zsv_ext0 = zsb + 1;
					}
					else if ((c1 & 0x02) == 0)
					{
						dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_3D;
						dy_ext0 = dy0 + 1 - SQUISH_CONSTANT_3D;
						dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_3D;
						xsv_ext0 = xsb + 1;
						ysv_ext0 = ysb - 1;
						zsv_ext0 = zsb + 1;
					}
					else
					{
						dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_3D;
						dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_3D;
						dz_ext0 = dz0 + 1 - SQUISH_CONSTANT_3D;
						xsv_ext0 = xsb + 1;
						ysv_ext0 = ysb + 1;
						zsv_ext0 = zsb - 1;
					}

					
					dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D;
					dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D;
					dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D;
					xsv_ext1 = xsb;
					ysv_ext1 = ysb;
					zsv_ext1 = zsb;
					if ((c2 & 0x01) != 0)
					{
						dx_ext1 -= 2;
						xsv_ext1 += 2;
					}
					else if ((c2 & 0x02) != 0)
					{
						dy_ext1 -= 2;
						ysv_ext1 += 2;
					}
					else
					{
						dz_ext1 -= 2;
						zsv_ext1 += 2;
					}
				}

				
				double dx1 = dx0 - 1 - SQUISH_CONSTANT_3D;
				double dy1 = dy0 - 0 - SQUISH_CONSTANT_3D;
				double dz1 = dz0 - 0 - SQUISH_CONSTANT_3D;
				double attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1;
				if (attn1 > 0)
				{
					attn1 *= attn1;
					value += attn1 * attn1 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, dx1, dy1, dz1);
				}

				
				double dx2 = dx0 - 0 - SQUISH_CONSTANT_3D;
				double dy2 = dy0 - 1 - SQUISH_CONSTANT_3D;
				double dz2 = dz1;
				double attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2;
				if (attn2 > 0)
				{
					attn2 *= attn2;
					value += attn2 * attn2 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, dx2, dy2, dz2);
				}

				
				double dx3 = dx2;
				double dy3 = dy1;
				double dz3 = dz0 - 1 - SQUISH_CONSTANT_3D;
				double attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3;
				if (attn3 > 0)
				{
					attn3 *= attn3;
					value += attn3 * attn3 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, dx3, dy3, dz3);
				}

				
				double dx4 = dx0 - 1 - 2 * SQUISH_CONSTANT_3D;
				double dy4 = dy0 - 1 - 2 * SQUISH_CONSTANT_3D;
				double dz4 = dz0 - 0 - 2 * SQUISH_CONSTANT_3D;
				double attn4 = 2 - dx4 * dx4 - dy4 * dy4 - dz4 * dz4;
				if (attn4 > 0)
				{
					attn4 *= attn4;
					value += attn4 * attn4 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, dx4, dy4, dz4);
				}

				
				double dx5 = dx4;
				double dy5 = dy0 - 0 - 2 * SQUISH_CONSTANT_3D;
				double dz5 = dz0 - 1 - 2 * SQUISH_CONSTANT_3D;
				double attn5 = 2 - dx5 * dx5 - dy5 * dy5 - dz5 * dz5;
				if (attn5 > 0)
				{
					attn5 *= attn5;
					value += attn5 * attn5 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, dx5, dy5, dz5);
				}

				
				double dx6 = dx0 - 0 - 2 * SQUISH_CONSTANT_3D;
				double dy6 = dy4;
				double dz6 = dz5;
				double attn6 = 2 - dx6 * dx6 - dy6 * dy6 - dz6 * dz6;
				if (attn6 > 0)
				{
					attn6 *= attn6;
					value += attn6 * attn6 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, dx6, dy6, dz6);
				}
			}

			
			double attn_ext0 = 2 - dx_ext0 * dx_ext0 - dy_ext0 * dy_ext0 - dz_ext0 * dz_ext0;
			if (attn_ext0 > 0)
			{
				attn_ext0 *= attn_ext0;
				value += attn_ext0 * attn_ext0 * Extrapolate(xsv_ext0, ysv_ext0, zsv_ext0, dx_ext0, dy_ext0, dz_ext0);
			}

			
			double attn_ext1 = 2 - dx_ext1 * dx_ext1 - dy_ext1 * dy_ext1 - dz_ext1 * dz_ext1;
			if (attn_ext1 > 0)
			{
				attn_ext1 *= attn_ext1;
				value += attn_ext1 * attn_ext1 * Extrapolate(xsv_ext1, ysv_ext1, zsv_ext1, dx_ext1, dy_ext1, dz_ext1);
			}

			return value / NORM_CONSTANT_3D;
		}
        private double Extrapolate(int xsb, int ysb, int zsb, double dx, double dy, double dz)
        {
            int index = permGradIndex3D[(perm[(perm[xsb & 0xFF] + ysb) & 0xFF] + zsb) & 0xFF];
            return gradients3D[index] * dx
                + gradients3D[index + 1] * dy
                + gradients3D[index + 2] * dz;
        }
        private static int FastFloor(double x)
        {
            int xi = (int)x;
            return x < xi ? xi - 1 : xi;
        }
    }
}
