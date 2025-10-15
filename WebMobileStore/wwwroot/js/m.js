const products = [
            
            {
                id: 2,
                name: "Samsung Galaxy S24 Ultra",
                price: 27990000,
                oldPrice: 32990000,
                discount: 15,
                image: "https://images.unsplash.com/photo-1610945415295-d9bbf067e59c?w=400",
                rating: 4.8,
                sold: 980
            },
            {
                id: 3,
                name: "MacBook Pro 14 M3",
                price: 39990000,
                oldPrice: 44990000,
                discount: 11,
                image: "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400",
                rating: 5.0,
                sold: 650
            },
            {
                id: 4,
                name: "AirPods Pro 2",
                price: 5490000,
                oldPrice: 6990000,
                discount: 21,
                image: "https://images.unsplash.com/photo-1606841837239-c5a1a4a07af7?w=400",
                rating: 4.9,
                sold: 2340
            },
            {
                id: 5,
                name: "iPad Air M2 11 inch",
                price: 14990000,
                oldPrice: 17990000,
                discount: 17,
                image: "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400",
                rating: 4.8,
                sold: 720
            },
            {
                id: 6,
                name: "Apple Watch Series 9",
                price: 9990000,
                oldPrice: 11990000,
                discount: 17,
                image: "https://images.unsplash.com/photo-1579586337278-3befd40fd17a?w=400",
                rating: 4.7,
                sold: 890
            },
            {
                id: 7,
                name: "Dell XPS 13 Plus",
                price: 32990000,
                oldPrice: 38990000,
                discount: 15,
                image: "https://images.unsplash.com/photo-1593642632823-8f785ba67e45?w=400",
                rating: 4.8,
                sold: 420
            },
            {
                id: 8,
                name: "Sony WH-1000XM5",
                price: 7490000,
                oldPrice: 8990000,
                discount: 17,
                image: "https://images.unsplash.com/photo-1618366712010-f4ae9c647dcf?w=400",
                rating: 4.9,
                sold: 1150
            }
        ];

        let cart = [];

        // Format price
        function formatPrice(price) {
            return price.toLocaleString('vi-VN') + 'đ';
        }

        // Render products
        function renderProducts(container, productList) {
            const html = productList.map(product => `
                <div class="product-card">
                    <span class="product-badge">-${product.discount}%</span>
                    <img src="${product.image}" alt="${product.name}" class="product-image">
                    <div class="product-name">${product.name}</div>
                    <div class="product-price">
                        <span class="price-current">${formatPrice(product.price)}</span>
                        <span class="price-old">${formatPrice(product.oldPrice)}</span>
                    </div>
                    <div class="product-rating">
                        ⭐ ${product.rating} | Đã bán ${product.sold}
                    </div>
                    <button class="add-to-cart-btn" onclick="addToCart(${product.id})">
                        Thêm vào giỏ
                    </button>
                </div>
            `).join('');
            document.getElementById(container).innerHTML = html;
        }

        // Add to cart
        function addToCart(productId) {
            const product = products.find(p => p.id === productId);
            const existingItem = cart.find(item => item.id === productId);
            
            if (existingItem) {
                existingItem.quantity++;
            } else {
                cart.push({ ...product, quantity: 1 });
            }
            
            updateCartCount();
            showNotification('Đã thêm vào giỏ hàng!');
        }

        // Update cart count
        function updateCartCount() {
            const count = cart.reduce((sum, item) => sum + item.quantity, 0);
            document.getElementById('cartCount').textContent = count;
        }

        // Open cart
        function openCart() {
            renderCart();
            document.getElementById('cartModal').style.display = 'flex';
        }

        // Close cart
        function closeCart() {
            document.getElementById('cartModal').style.display = 'none';
        }

        // Render cart
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

        // Show notification
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

        // Initialize
        renderProducts('productsContainer', products.slice(0, 4));
        renderProducts('recommendedContainer', products.slice(4));

        // Flash sale countdown
        setInterval(() => {
            const timer = document.querySelector('.flash-sale-timer');
            const parts = timer.textContent.split(': ')[1].split(':');
            let [h, m, s] = parts.map(Number);
            
            s--;
            if (s < 0) { s = 59; m--; }
            if (m < 0) { m = 59; h--; }
            if (h < 0) h = 0;
            
            timer.textContent = `Kết thúc sau: ${String(h).padStart(2,'0')}:${String(m).padStart(2,'0')}:${String(s).padStart(2,'0')}`;
        }, 1000);


  