const getCartUrl = "/Cart/GetCart";
const addToCartUrl = "/Cart/AddToCart";
const updateUrl = "/Cart/UpdateQuantity";
const deleteUrl = "/Cart/DeleteItem";
const clearUrl = "/Cart/Clear";

//////////////////////////
// ctr f5 neu khong load
////////////////////////


function showToast(message, timeout = 2000) {
    const t = document.getElementById('cartToast');
    t.textContent = message;
    t.style.display = 'block';
    t.style.opacity = '1';
    setTimeout(() => {
        t.style.opacity = '0';
        setTimeout(() => t.style.display = 'none', 300);
    }, timeout);
}

function formatPrice(price) {
    return Number(price).toLocaleString('vi-VN') + 'đ';
}

function renderCart(items) {
    let html = '';
    let subtotal = 0;

    items.forEach(it => {
        const line = it.Quantity * it.Price;
        subtotal += line;
        html += `
            <div class="cart-item" data-id="${it.CartItemId}">
                <img src="${it.ImageUrl}" class="cart-thumb" />
                <div class="cart-info">
                    <div class="cart-name">${it.ProductName}</div>
                    <small class="cart-variant">${it.VariantName}</small>
                </div>
                <div class="qty-box">
                    <button class="qty-btn minus" data-id="${it.CartItemId}">-</button>
                    <span class="qty" id="qty-${it.CartItemId}">${it.Quantity}</span>
                    <button class="qty-btn plus" data-id="${it.CartItemId}">+</button>
                </div>
                <div class="cart-price" id="price-${it.CartItemId}">${formatPrice(line)}</div>
                <button class="delete-item" data-id="${it.CartItemId}">🗑</button>
            </div>
        `;
    });

    document.getElementById('cartItems').innerHTML = html;
    document.getElementById('subtotal').textContent = formatPrice(subtotal);
    document.getElementById('totalPrice').textContent = formatPrice(subtotal + 30000);

    // **Attach sự kiện nút + / - / xoá ở đây**
    document.querySelectorAll('.qty-btn.plus').forEach(btn => btn.addEventListener('click', onPlus));
    document.querySelectorAll('.qty-btn.minus').forEach(btn => btn.addEventListener('click', onMinus));
    document.querySelectorAll('.delete-item').forEach(btn => btn.addEventListener('click', e => deleteItem(e.currentTarget.dataset.id)));
}



// Open cart: fetch and render
function openCart() {
    console.log("Cart opened!");
    const modal = document.getElementById("cartModal");
    modal.style.display = "block";

    fetch("/Cart/GetCart")
        .then(res => res.json())
        .then(data => {
            const cartItemsEl = document.getElementById("cartItems");
            if (!data.success || !data.cartItems || data.cartItems.length === 0) {
                cartItemsEl.innerHTML = `<p style="text-align:center;color:#777;">Giỏ hàng trống</p>`;
                document.getElementById("subtotal").textContent = "0đ";
                document.getElementById("totalPrice").textContent = "0đ";
                return;
            }

            let html = "";
            let subtotal = 0;

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

            cartItemsEl.innerHTML = html;
            document.getElementById("subtotal").textContent = formatPrice(subtotal);
            document.getElementById("totalPrice").textContent = formatPrice(subtotal + 30000);

            // Attach sự kiện cho nút + / - / xoá
            document.querySelectorAll('.qty-btn.plus').forEach(btn => btn.addEventListener('click', onPlus));
            document.querySelectorAll('.qty-btn.minus').forEach(btn => btn.addEventListener('click', onMinus));
            document.querySelectorAll('.delete-item').forEach(btn => btn.addEventListener('click', e => deleteItem(e.currentTarget.dataset.id)));
        })
        .catch(err => {
            console.error(err);
            document.getElementById("cartItems").innerHTML =
                `<p style="text-align:center;color:red;">Lỗi khi tải giỏ hàng</p>`;
        });
}



function onPlus(e) {
    const id = e.currentTarget.dataset.id;
    const qtyEl = document.getElementById(`qty-${id}`);
    let next = parseInt(qtyEl.textContent) + 1;
    updateQuantity(id, next);
}

function onMinus(e) {
    const id = e.currentTarget.dataset.id;
    const qtyEl = document.getElementById(`qty-${id}`);
    let next = parseInt(qtyEl.textContent) - 1;
    if (next <= 0) {
        // theo lựa chọn A: nếu giảm dưới 1 thì xóa item
        if (confirm('Bạn muốn xoá sản phẩm này khỏi giỏ hàng?')) {
            deleteItem(id);
        }
        return;
    }
    updateQuantity(id, next);
}

function updateQuantity(cartItemId, quantity) {
    fetch(updateUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `cartItemId=${cartItemId}&quantity=${quantity}`
    }).then(r => r.json()).then(res => {
        if (!res.success) { showToast(res.message || 'Lỗi'); return; }
        // reload cart UI
        openCart();
        document.getElementById('cartCount').textContent = res.cartCount;
        showToast('Cập nhật thành công');
    });
}

function deleteItem(cartItemId) {
    fetch(deleteUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `cartItemId=${cartItemId}`
    }).then(r => r.json()).then(res => {
        if (!res.success) { showToast(res.message || 'Lỗi'); return; }
        openCart();
        document.getElementById('cartCount').textContent = res.cartCount;
        showToast(res.message || 'Đã xoá');
    });
}

// Clear all
document.addEventListener('click', function (e) {
    if (e.target && e.target.matches('.clear-cart-btn')) {
        if (!confirm('Bạn có chắc muốn xóa toàn bộ giỏ hàng?')) return;
        fetch(clearUrl, { method: 'POST' })
            .then(r => r.json())
            .then(res => {
                if (!res.success) { showToast(res.message || 'Lỗi'); return; }
                openCart();
                document.getElementById('cartCount').textContent = res.cartCount || 0;
                showToast(res.message || 'Đã xóa toàn bộ giỏ hàng');
            });
    }
});


function onMinus(e) {
    const id = e.currentTarget.dataset.id;
    const qtyEl = document.getElementById(`qty-${id}`);
    let next = parseInt(qtyEl.textContent) - 1;
    if (next <= 0) {
        if (confirm("Bạn muốn xóa sản phẩm khỏi giỏ hàng?")) deleteItem(id);
        return;
    }
    updateQuantity(id, next);
}

function updateQuantity(cartItemId, quantity) {
    fetch(updateUrl, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: `cartItemId=${cartItemId}&quantity=${quantity}`
    }).then(res => res.json()).then(res => {
        if (!res.success) return alert(res.message);
        openCart(); // reload cart
    });
}


// Add to cart helper
function addToCart(variantId, qty = 1) {
    fetch(addToCartUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `variantId=${variantId}&quantity=${qty}`
    }).then(r => r.json()).then(res => {
        if (!res.success) { showToast(res.message || 'Lỗi'); return; }
        document.getElementById('cartCount').textContent = res.cartCount;
        showToast(res.message || 'Đã thêm vào giỏ hàng');
    });
}

// init: update cart count on page load
function updateCartCount() {
    fetch(getCartUrl).then(r => r.json()).then(res => {
        if (res.success) {
            const count = res.cartItems.reduce((s, it) => s + it.Quantity, 0);
            document.getElementById('cartCount').textContent = count;
        }
    });
}



function closeCart() {
    document.getElementById("cartModal").style.display = "none";
}


document.addEventListener("DOMContentLoaded", function () {
    updateCartCount();
});

window.openCart = openCart;
window.closeCart = closeCart;

