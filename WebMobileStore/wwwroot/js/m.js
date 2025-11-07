const urls = {
    getCart: "/Cart/GetCart",
    addToCart: "/Cart/AddToCart",
    updateQty: "/Cart/UpdateQuantity",
    deleteItem: "/Cart/DeleteItem",
    clear: "/Cart/Clear"
};

function formatPrice(price) {
    return Number(price).toLocaleString('vi-VN') + 'đ';
}

function showToast(message, timeout = 2000) {
    const t = document.getElementById('cartToast');
    if (!t) return alert(message);
    t.textContent = message;
    t.style.display = 'block';
    t.style.opacity = '1';
    setTimeout(() => {
        t.style.opacity = '0';
        setTimeout(() => t.style.display = 'none', 300);
    }, timeout);
}

// ==================== GIỎ HÀNG ====================

function openCart() {
    const modal = document.getElementById("cartModal");
    modal.style.display = "block";

    fetch(urls.getCart)
        .then(res => res.json())
        .then(data => {
            const el = document.getElementById("cartItems");
            if (!data.success || !data.cartItems || !data.cartItems.length) {
                el.innerHTML = `<p style="text-align:center;color:#777;">Giỏ hàng trống</p>`;
                document.getElementById("subtotal").textContent = "0đ";
                document.getElementById("totalPrice").textContent = "0đ";
                return;
            }

            let html = "", subtotal = 0;
            data.cartItems.forEach(it => {
                const line = it.quantity * it.price;
                subtotal += line;
                html += `
                    <div class="cart-item" data-id="${it.cartItemId}">
                        <img src="${it.imageUrl}" class="cart-thumb" />
                        <div class="cart-info">
                            <div class="cart-name">${it.productName}</div>
                            <small class="cart-variant">${it.variantName}</small>
                        </div>
                        <div class="qty-box">
                            <button class="qty-btn minus" data-id="${it.cartItemId}">-</button>
                            <span class="qty" id="qty-${it.cartItemId}">${it.quantity}</span>
                            <button class="qty-btn plus" data-id="${it.cartItemId}">+</button>
                        </div>
                        <div class="cart-price" id="price-${it.cartItemId}">${formatPrice(line)}</div>
                        <button class="delete-item" data-id="${it.cartItemId}">🗑</button>
                    </div>
                `;
            });

            el.innerHTML = html;
            document.getElementById("subtotal").textContent = formatPrice(subtotal);
            document.getElementById("totalPrice").textContent = formatPrice(subtotal + 30000);

            document.querySelectorAll('.qty-btn.plus').forEach(b => b.onclick = () => changeQty(b.dataset.id, +1));
            document.querySelectorAll('.qty-btn.minus').forEach(b => b.onclick = () => changeQty(b.dataset.id, -1));
            document.querySelectorAll('.delete-item').forEach(b => b.onclick = () => deleteItem(b.dataset.id));
        })
        .catch(() => {
            document.getElementById("cartItems").innerHTML =
                `<p style="text-align:center;color:red;">Lỗi khi tải giỏ hàng</p>`;
        });
}

function closeCart() {
    document.getElementById("cartModal").style.display = "none";
}

function updateCartCount() {
    fetch(urls.getCart)
        .then(r => r.json())
        .then(res => {
            if (res.success) {
                const count = res.cartItems.reduce((s, it) => s + it.quantity, 0);
                document.getElementById('cartCount').textContent = count;
            }
        });
}


function addToCart(variantId, qty = 1) {
    fetch(urls.addToCart, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `variantId=${variantId}&quantity=${qty}`
    })
    .then(r => r.json())
    .then(res => {
        if (!res.success) return showToast(res.message || 'Lỗi khi thêm sản phẩm');
        updateCartCount();
        showToast('Đã thêm vào giỏ hàng');
    })
    .catch(() => showToast('Không thể thêm vào giỏ hàng'));
}

function changeQty(id, diff) {
    const qtyEl = document.getElementById(`qty-${id}`);
    let next = parseInt(qtyEl.textContent) + diff;
    if (next <= 0) {
        if (confirm('Bạn có muốn xoá sản phẩm này?')) deleteItem(id);
        return;
    }
    updateQuantity(id, next);
}

function updateQuantity(cartItemId, quantity) {
    fetch(urls.updateQty, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `cartItemId=${cartItemId}&quantity=${quantity}`
    })
    .then(r => r.json())
    .then(res => {
        if (!res.success) return showToast(res.message || 'Lỗi cập nhật');
        openCart();
        updateCartCount();
        showToast('Cập nhật thành công');
    });
}

function deleteItem(cartItemId) {
    fetch(urls.deleteItem, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `cartItemId=${cartItemId}`
    })
    .then(r => r.json())
    .then(res => {
        if (!res.success) return showToast(res.message || 'Lỗi khi xoá');
        openCart();
        updateCartCount();
        showToast('Đã xoá sản phẩm');
    });
}

document.addEventListener('click', function (e) {
    if (e.target.matches('.clear-cart-btn')) {
        if (!confirm('Bạn muốn xóa toàn bộ giỏ hàng?')) return;
        fetch(urls.clear, { method: 'POST' })
            .then(r => r.json())
            .then(res => {
                if (!res.success) return showToast(res.message || 'Lỗi xoá toàn bộ');
                openCart();
                updateCartCount();
                showToast('Đã xóa toàn bộ giỏ hàng');
            });
    }
});



document.addEventListener("DOMContentLoaded", updateCartCount);
window.openCart = openCart;
window.closeCart = closeCart;

///////////////////////////////////////////////////////

