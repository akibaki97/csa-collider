using Model.MathAndGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathAndGeometry
{
    class CPolynomial
    {
        CPolynomial(Complex[] p)
        {
            N = p.Length;
            this.p = p;
        }
        
        CPolynomial(TrigPoly trigPoly)
        {

        }

        Complex[] p;
        int N;

        Complex[] AberthEhrlich()
        {
            return null;
        }

    }
}
