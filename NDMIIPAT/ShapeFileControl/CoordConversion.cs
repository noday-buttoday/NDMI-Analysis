using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeFileControl
{
    public class LatLon
    {
        public double lat{get;set;}
        public double lon{get;set;}
    }

    class CoordConversion
    {
        private double asinz(double value)
        {
            if (Math.Abs(value) > 1.0)
                value = (value > 0 ? 1 : -1);

            return Math.Asin(value);
        }

        private double e0fn(double x)
        {
	        return 1.0 - 0.25 * x * (1.0 + x / 16.0 * (3.0 + 1.25 * x));
        }

        private double e1fn(double x)
        {
	        return 0.375 * x * (1.0 + 0.25 * x * (1.0 + 0.46875 * x));
        }

        private double e2fn(double x)
        {
	        return 0.05859375 * x * x * (1.0 + 0.75 * x);
        }

        private double e3fn(double x)
        {
	        return x * x * x * (35.0 / 3072.0);
        }

        private double e4fn(double x)
        {
	        double con, com;

            con = 1.0 + x;
            com = 1.0 - x;
            return Math.Sqrt(Math.Pow(con, con) * Math.Pow(com, com));
        }

        double mlfn(double e0, double e1, double e2, double e3, double phi)
        {
            return e0 * phi - e1 * Math.Sin(2.0 * phi) + e2 * Math.Sin(4.0 * phi) - e3 * Math.Sin(6.0 * phi);
        }

        public LatLon Tm2Geo(double x, double y)
        {
            LatLon result = new LatLon();
            double lat, lon;

            double con; // temporary angles
            double phi; // temporary angles
            double delta_Phi; // difference between longitudes
            long i; // counter variable
            double sin_phi, cos_phi, tan_phi; // sin cos and tangent values
            double c, cs, t, ts, n, r, d, ds; // temporary variables
            double f, h, g, temp; // temporary variables

            //double m_arScaleFactor = 1;
            //double m_arLonCenter = 2.216568150032799;
            //double m_arLatCenter = 0.663225115757845;

            double m_arScaleFactor = 0.9996;
            double m_arLonCenter = 127.5 * 0.0174532925199433;
            double m_arLatCenter = 38.0 * 0.0174532925199433;

            double m_arFalseNorthing = 2000000.0;
            double m_arFalseEasting = 1000000.0;

            #region BESSEL_SPHEROID
            //double m_arMajor = 6377397.155;
            //double m_arMinor = 6356078.96325;
            #endregion
            #region GRS80_SPHEROID
            double m_arMajor = 6378137.000;
            double m_arMinor = 6356752.3142;
            #endregion
            double m_dSrcInd = 1.0;

            double a = m_arMinor / m_arMajor;
            double m_dSrcEs = 1.0 - a * a;
            double m_dSrcE = Math.Sqrt(m_dSrcEs);
            double m_dSrcE0 = e0fn(m_dSrcEs);
            double m_dSrcE1 = e1fn(m_dSrcEs);
            double m_dSrcE2 = e2fn(m_dSrcEs);
            double m_dSrcE3 = e3fn(m_dSrcEs);
            double m_dSrcMl0 = m_arMajor * mlfn(m_dSrcE0, m_dSrcE1, m_dSrcE2, m_dSrcE3, m_arLatCenter);
            double m_dSrcEsp = m_dSrcEs / (1.0 - m_dSrcEs);

            const double EPSLN = 0.0000000001;

            if (m_dSrcInd != 0)
            {
                f = Math.Exp(x / (m_arMajor * m_arScaleFactor));
                g = 0.5 * (f - 1.0 / f);
                temp = m_arLatCenter + y / (m_arMajor * m_arScaleFactor);
                h = Math.Cos(temp);
                con = Math.Sqrt((1.0 - h * h) / (1.0 + g * g));
                lat = asinz(con);

                if (temp < 0)
                    lat *= -1;

                if ((g == 0) && (h == 0))
                    lon = m_arLonCenter;
                else
                    lon = Math.Atan(g / h) + m_arLonCenter;
            }

            // TM to LL inverse equations from here
            x -= m_arFalseEasting;
            y -= m_arFalseNorthing;

            con = (m_dSrcMl0 + y / m_arScaleFactor) / m_arMajor;
            phi = con;

            i = 0;
            while (true)
            {
                delta_Phi = ((con + m_dSrcE1 * Math.Sin(2.0 * phi) - m_dSrcE2 * Math.Sin(4.0 * phi) + m_dSrcE3 * Math.Sin(6.0 * phi)) / m_dSrcE0) - phi;
                phi = phi + delta_Phi;
                if (Math.Abs(delta_Phi) <= EPSLN) break;

                if (i > 6)
                    break;

                i++;
            }

            if (Math.Abs(phi) < (Math.PI / 2))
            {
                sin_phi = Math.Sin(phi);
                cos_phi = Math.Cos(phi);
                tan_phi = Math.Tan(phi);
                c = m_dSrcEsp * cos_phi * cos_phi;
                cs = c * c;
                t = tan_phi * tan_phi;
                ts = t * t;
                con = 1.0 - m_dSrcEs * sin_phi * sin_phi;
                n = m_arMajor / Math.Sqrt(con);
                r = n * (1.0 - m_dSrcEs) / con;
                d = x / (n * m_arScaleFactor);
                ds = d * d;
                lat = phi - (n * tan_phi * ds / r) * (0.5 - ds / 24.0 * (5.0 + 3.0 * t + 10.0 * c - 4.0 * cs - 9.0 * m_dSrcEsp - ds / 30.0 * (61.0 + 90.0 * t + 298.0 * c + 45.0 * ts - 252.0 * m_dSrcEsp - 3.0 * cs)));
                lon = m_arLonCenter + (d * (1.0 - ds / 6.0 * (1.0 + 2.0 * t + c - ds / 20.0 * (5.0 - 2.0 * c + 28.0 * t - 3.0 * cs + 8.0 * m_dSrcEsp + 24.0 * ts))) / cos_phi);
            }
            else
            {
                lat = Math.PI * 0.5 * Math.Sin(y);
                lon = m_arLonCenter;
            }

            result.lat = lat;
            result.lon = lon;

            return result;
        }
    }
}
