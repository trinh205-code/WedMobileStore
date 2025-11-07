namespace WebMobileStore.Models.Entity.Enums
{
    public enum PaymentMethod
    {
        CashOnDelivery = 0,  // Thanh toán khi nhận hàng
        BankTransfer = 1,    // Chuyển khoản ngân hàng
        CreditCard = 2,      // Thẻ tín dụng (Visa/MasterCard)
        EWallet = 3,         // Ví điện tử (Momo, ZaloPay, ...)

    }
}
