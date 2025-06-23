namespace Ecommerce_brand_Api.Models.Entities
{
        public class Discount
        {
                public int Id { get; set; }
                public decimal Threshold { get; set; } // 200 if (totalorderprice > threshold){
                                                       //  orderdtodicount value = Discount.Dicount
                                                       // }
                public decimal DicountValue { get; set; }//50
  }

        
}
