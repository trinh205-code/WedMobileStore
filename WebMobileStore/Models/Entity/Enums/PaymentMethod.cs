namespace WebMobileStore.Models.Entity.Enums
{
    public enum PaymentMethod
    {
        CashOnDelivery = 0,  // Thanh toán khi nhận hàng
        BankTransfer = 1,    // Chuyển khoản ngân hàng
        CreditCard = 2,      // Thẻ tín dụng (Visa/MasterCard)
        DebitCard = 3,       // Thẻ ghi nợ nội địa
        EWallet = 4,         // Ví điện tử (Momo, ZaloPay, ...)
        PayPal = 5,          // Thanh toán quốc tế

    }
}
