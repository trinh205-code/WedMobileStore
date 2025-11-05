let cart = [];

// Format price
function formatPrice(price) {
    return price.toLocaleString('vi-VN') + 'đ';
}

// Add to cart
function addToCart(productId) {
    // TODO: gọi API backend hoặc localStorage
    showNotification('Đã thêm vào giỏ hàng!');
}

// Update cart count
function updateCartCount() {
    const count = cart.reduce((sum, item) => sum + item.quantity, 0);
    document.getElementById('cartCount').textContent = count;
}

// Open cart modal
function openCart() {
    renderCart();
    document.getElementById('cartModal').style.display = 'flex';
}

// Close cart
function closeCart() {
    document.getElementById('cartModal').style.display = 'none';
}

// Render cart (client-side local version)
function renderCart() {
    const cartItems = document.getElementById('cartItems');

    if (cart.length === 0) {
        cartItems.innerHTML = '<p style="text-align:center;padding:20px;">Giỏ hàng trống</p>';
        document.getElementById('totalPrice').textContent = '0đ';
        return;
    }

    const html = cart.map(item => `
        <div class="cart-item">
            <img src="${item.image}" alt="${item.name}">
            <div class="cart-item-info">
                <div style="font-weight:bold;margin-bottom:5px;">${item.name}</div>
                <div style="color:#ff4757;font-weight:bold;">${formatPrice(item.price)}</div>
                <div>Số lượng: ${item.quantity}</div>
            </div>
        </div>
    `).join('');

    cartItems.innerHTML = html;

    const total = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    document.getElementById('totalPrice').textContent = formatPrice(total);
}

// Notification popup
function showNotification(message) {
    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed;
        top: 100px;
        right: 20px;
        background: #10ac84;
        color: white;
        padding: 15px 25px;
        border-radius: 8px;
        z-index: 3000;
        animation: slideIn 0.3s ease;
    `;
    notification.textContent = message;
    document.body.appendChild(notification);

    setTimeout(() => {
        notification.remove();
    }, 2000);
}
