// Sample products data
const products = [
    {
        id: 1,
        name: "iPhone 15 Pro Max 256GB",
        price: 29990000,
        oldPrice: 34990000,
        category: "Điện thoại",
        brand: "Apple",
        stock: 45,
        image: "https://images.unsplash.com/photo-1592286927505-c0d0e0d7b0b8?w=400"
    },
    {
        id: 2,
        name: "Samsung Galaxy S24 Ultra",
        price: 27990000,
        oldPrice: 32990000,
        category: "Điện thoại",
        brand: "Samsung",
        stock: 32,
        image: "https://images.unsplash.com/photo-1610945415295-d9bbf067e59c?w=400"
    },
    {
        id: 3,
        name: "MacBook Pro 14 M3",
        price: 39990000,
        oldPrice: 44990000,
        category: "Laptop",
        brand: "Apple",
        stock: 18,
        image: "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400"
    },
    {
        id: 4,
        name: "AirPods Pro 2",
        price: 5490000,
        oldPrice: 6990000,
        category: "Phụ kiện",
        brand: "Apple",
        stock: 120,
        image: "https://images.unsplash.com/photo-1606841837239-c5a1a4a07af7?w=400"
    },
    {
        id: 5,
        name: "iPad Air M2 11 inch",
        price: 14990000,
        oldPrice: 17990000,
        category: "Tablet",
        brand: "Apple",
        stock: 25,
        image: "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400"
    },
    {
        id: 6,
        name: "Dell XPS 13 Plus",
        price: 32990000,
        oldPrice: 38990000,
        category: "Laptop",
        brand: "Dell",
        stock: 15,
        image: "https://images.unsplash.com/photo-1593642632823-8f785ba67e45?w=400"
    }
];

let selectedImages = [];

// Format price
function formatPrice(price) {
    return price.toLocaleString('vi-VN') + '₫';
}

// Render products grid
function renderProducts() {
    const grid = document.getElementById('productsGrid');
    if (!grid) return;
    
    const html = products.map(product => `
        <div class="product-management-card">
            <img src="${product.image}" alt="${product.name}">
            <h3>${product.name}</h3>
            <div class="price">${formatPrice(product.price)}</div>
            <div class="stock">Còn ${product.stock} sản phẩm</div>
            <div class="product-actions">
                <button class="btn-edit" onclick="editProduct(${product.id})">✏️ Sửa</button>
                <button class="btn-delete" onclick="deleteProduct(${product.id})">🗑️ Xóa</button>
            </div>
        </div>
    `).join('');
    grid.innerHTML = html;
}

// Show page
function showPage(pageName) {
    // Hide all pages
    document.querySelectorAll('.page-content').forEach(page => {
        page.classList.remove('active');
    });
    
    // Show selected page
    const page = document.getElementById('page-' + pageName);
    if (page) {
        page.classList.add('active');
    }
    
    // Update active menu
    document.querySelectorAll('.menu-item').forEach(item => {
        item.classList.remove('active');
        if (item.dataset.page === pageName) {
            item.classList.add('active');
        }
    });

    // Update breadcrumb
    const breadcrumb = document.querySelector('.breadcrumb');
    const pageNames = {
        'dashboard': 'Dashboard',
        'products': 'Danh sách sản phẩm',
        'add-product': 'Thêm sản phẩm',
        'categories': 'Danh mục',
        'orders': 'Đơn hàng',
        'customers': 'Khách hàng',
        'settings': 'Cài đặt'
    };
    breadcrumb.innerHTML = `
        <a href="#" onclick="showPage('dashboard')">Home</a>
        <span>/</span>
        <span>${pageNames[pageName] || 'Dashboard'}</span>
    `;

    // Load products if on products page
    if (pageName === 'products') {
        renderProducts();
    }

    // Reset form title when going to add product
    if (pageName === 'add-product') {
        const title = document.querySelector('#page-add-product h2');
        if (title) {
            title.textContent = 'Thêm sản phẩm mới';
        }
        const form = document.getElementById('addProductForm');
        if (form) {
            form.reset();
        }
        const preview = document.getElementById('imagePreview');
        if (preview) {
            preview.innerHTML = '';
        }
        selectedImages = [];
    }
}

// Toggle Sidebar
function toggleSidebar() {
    document.getElementById('sidebar').classList.toggle('open');
}

// Toggle Dark Mode
function toggleDarkMode() {
    document.body.classList.toggle('dark-mode');
    localStorage.setItem('darkMode', document.body.classList.contains('dark-mode'));
}

// Load dark mode preference
function loadDarkMode() {
    if (localStorage.getItem('darkMode') === 'true') {
        document.body.classList.add('dark-mode');
    }
}

// Preview images
function previewImages(event) {
    const files = Array.from(event.target.files);
    const preview = document.getElementById('imagePreview');
    
    files.forEach(file => {
        if (file.type.startsWith('image/')) {
            const reader = new FileReader();
            reader.onload = function(e) {
                const div = document.createElement('div');
                div.className = 'preview-item';
                div.innerHTML = `
                    <img src="${e.target.result}" alt="Preview">
                    <button class="remove-image" onclick="this.parentElement.remove()">×</button>
                `;
                preview.appendChild(div);
                selectedImages.push(e.target.result);
            };
            reader.readAsDataURL(file);
        }
    });
}

// Handle add product form
function handleAddProduct(event) {
    event.preventDefault();
    const form = event.target;
    
    // Get form data
    const productName = form.querySelector('input[type="text"]').value;
    const category = form.querySelector('.form-select').value;
    const price = form.querySelectorAll('input[type="number"]')[0].value;
    const oldPrice = form.querySelectorAll('input[type="number"]')[1].value;
    const stock = form.querySelectorAll('input[type="number"]')[2].value;

    // Create new product
    const newProduct = {
        id: products.length + 1,
        name: productName,
        price: parseInt(price),
        oldPrice: parseInt(oldPrice),
        category: category,
        brand: form.querySelectorAll('.form-select')[1].value,
        stock: parseInt(stock),
        image: selectedImages[0] || "https://via.placeholder.com/400"
    };

    // Add to products array
    products.push(newProduct);
    
    // Show success message
    alert('Sản phẩm đã được thêm thành công!');
    
    // Reset form
    form.reset();
    document.getElementById('imagePreview').innerHTML = '';
    selectedImages = [];
    
    // Go back to products page
    showPage('products');
}

// Edit product
function editProduct(id) {
    const product = products.find(p => p.id === id);
    if (product) {
        showPage('add-product');
        // Pre-fill form with product data
        setTimeout(() => {
            const form = document.getElementById('addProductForm');
            if (form) {
                form.querySelector('input[type="text"]').value = product.name;
                form.querySelectorAll('input[type="number"]')[0].value = product.price;
                form.querySelectorAll('input[type="number"]')[1].value = product.oldPrice;
                form.querySelectorAll('input[type="number"]')[2].value = product.stock;
                
                // Change form title
                document.querySelector('#page-add-product h2').textContent = 'Chỉnh sửa sản phẩm';
            }
        }, 100);
    }
}

// Delete product
function deleteProduct(id) {
    if (confirm('Bạn có chắc chắn muốn xóa sản phẩm này?')) {
        const index = products.findIndex(p => p.id === id);
        if (index > -1) {
            products.splice(index, 1);
            renderProducts();
            alert('Sản phẩm đã được xóa!');
        }
    }
}

// Search product
function initSearchProduct() {
    const searchInput = document.getElementById('searchProduct');
    if (searchInput) {
        searchInput.addEventListener('input', function(e) {
            const searchTerm = e.target.value.toLowerCase();
            const filteredProducts = products.filter(p => 
                p.name.toLowerCase().includes(searchTerm) ||
                p.brand.toLowerCase().includes(searchTerm)
            );
            
            const grid = document.getElementById('productsGrid');
            if (filteredProducts.length === 0) {
                grid.innerHTML = '<p style="text-align:center;padding:40px;color:#8a93a2;">Không tìm thấy sản phẩm</p>';
            } else {
                const html = filteredProducts.map(product => `
                    <div class="product-management-card">
                        <img src="${product.image}" alt="${product.name}">
                        <h3>${product.name}</h3>
                        <div class="price">${formatPrice(product.price)}</div>
                        <div class="stock">Còn ${product.stock} sản phẩm</div>
                        <div class="product-actions">
                            <button class="btn-edit" onclick="editProduct(${product.id})">✏️ Sửa</button>
                            <button class="btn-delete" onclick="deleteProduct(${product.id})">🗑️ Xóa</button>
                        </div>
                    </div>
                `).join('');
                grid.innerHTML = html;
            }
        });
    }
}

// Simulate real-time data updates for dashboard
function startDashboardUpdates() {
    setInterval(() => {
        const dashboardPage = document.getElementById('page-dashboard');
        if (dashboardPage && dashboardPage.classList.contains('active')) {
            const values = document.querySelectorAll('#page-dashboard .stat-value');
            values.forEach(val => {
                const current = parseInt(val.textContent.replace(/[^0-9]/g, ''));
                const change = Math.floor(Math.random() * 100) - 50;
                const newValue = Math.max(0, current + change);
                
                if (val.textContent.includes('K')) {
                    val.textContent = (newValue / 1000).toFixed(0) + 'K';
                } else if (val.textContent.includes('$')) {
                    val.textContent = '$' + newValue.toLocaleString();
                } else if (val.textContent.includes('%')) {
                    val.textContent = (newValue / 100).toFixed(2) + '%';
                }
            });
        }
    }, 5000);
}

// Initialize menu items
function initMenuItems() {
    document.querySelectorAll('.menu-item').forEach(item => {
        item.addEventListener('click', function() {
            const page = this.dataset.page;
            if (page) {
                showPage(page);
            }
        });
    });
}

// Initialize filter buttons
function initFilterButtons() {
    document.querySelectorAll('.filter-btn').forEach(btn => {
        btn.addEventListener('click', function() {
            if (this.textContent !== '🔄') {
                document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
                this.classList.add('active');
            }
        });
    });
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', function() {
    // Load preferences
    loadDarkMode();
    
    // Initialize components
    initMenuItems();
    initFilterButtons();
    initSearchProduct();
    renderProducts();
    startDashboardUpdates();
    
    console.log('Admin Dashboard initialized successfully!');
});


function addVariant() {
    const container = document.getElementById('variantsContainer');
    const index = container.children.length;
    const div = document.createElement('div');
    div.className = 'variant-row';
    div.innerHTML = `
        <input type="text" name="variants[${index}].color" placeholder="Màu sắc" class="form-input" required>
        <input type="text" name="variants[${index}].capacity" placeholder="Dung lượng (VD: 128GB)" class="form-input" required>
        <input type="number" name="variants[${index}].price" placeholder="Giá bán" class="form-input" required>
        <input type="number" name="variants[${index}].stock" placeholder="Số lượng" class="form-input" required>
        <button type="button" class="remove-variant-btn" onclick="this.parentElement.remove()">❌</button>
    `;
    container.appendChild(div);
}

function loadPage(pageName) {
    if (pageName === "Index") return; // ngăn vòng lặp

    $.ajax({
        url: '/Admin/' + pageName,
        type: 'GET',
        success: function (data) {
            $('#mainContent').html(data);
        },
        error: function () {
            alert('Không thể tải trang!');
        }
    });
}

// Toggle Sidebar
function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    sidebar.classList.toggle('collapsed');
}

// Auto hide alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function () {
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.opacity = '0';
            alert.style.transform = 'translateY(-20px)';
            setTimeout(() => {
                alert.remove();
            }, 300);
        }, 5000);
    });
});

// Mobile sidebar toggle
if (window.innerWidth <= 768) {
    document.querySelector('.menu-toggle').addEventListener('click', function () {
        document.getElementById('sidebar').classList.toggle('active');
    });
}


//function loadPage(pageName) {
//    // Xóa active class khỏi tất cả menu items
//    $('.menu-item').removeClass('active');

//    // Thêm active class cho menu được click
//    event.currentTarget.classList.add('active');

//    // Load PartialView bằng AJAX
//    $.ajax({
//        url: '/Admin/' + pageName,
//        type: 'GET',
//        success: function (data) {
//            $('#mainContent').html(data);
//        },
//        error: function () {
//            alert('Không thể tải trang!');
//        }
//    });
//}
