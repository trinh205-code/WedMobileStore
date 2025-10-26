document.getElementById("variantForm").addEventListener("submit", function (e) {
    e.preventDefault(); // Ngăn submit ngay lập tức

    const variantsData = document.getElementById("variantsData");
    variantsData.innerHTML = "";
    let index = 0;
    const productId = document.querySelector("input[name='ProductId']").value;

    document.querySelectorAll(".config-row").forEach(row => {
        const storage = row.querySelector(".storage-select").value;
        const quantity = row.querySelector(".quantity-input").value;
        const comparePrice = row.querySelector(".compare-price").value || "0";
        const sellPrice = row.querySelector(".sell-price").value;
        const checkedColors = row.querySelectorAll(".color-input:checked");

        if (!storage) {
            alert("Vui lòng chọn dung lượng!");
            return;
        }

        if (checkedColors.length === 0) {
            alert("Vui lòng chọn ít nhất 1 màu!");
            return;
        }

        checkedColors.forEach(color => {
            variantsData.insertAdjacentHTML("beforeend", `
                <input type="hidden" name="Variants[${index}].ProductId" value="${productId}">
                <input type="hidden" name="Variants[${index}].Storage" value="${storage}">
                <input type="hidden" name="Variants[${index}].Color" value="${color.value}">
                <input type="hidden" name="Variants[${index}].Quantity" value="${quantity}">
                <input type="hidden" name="Variants[${index}].CompareAtPrice" value="${comparePrice}">
                <input type="hidden" name="Variants[${index}].Price" value="${sellPrice}">
            `);
            index++;
        });
    });

    if (index === 0) {
        alert("Bạn cần chọn ít nhất 1 biến thể!");
        return;
    }

    // Submit form sau khi đã tạo xong hidden inputs
    this.submit();
});