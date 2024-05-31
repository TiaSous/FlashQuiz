using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashQuiz.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string? Terme { get; set; }
        public string? Definition { get; set; }

        // permet de lorsqu'on fait un console write, cela va utiliser automatiquement la méthode tostring
        public override string ToString()
        {
            return $"[Card {Id}]";
        }
    }
}
