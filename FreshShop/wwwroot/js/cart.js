document.querySelectorAll('.quantity-box input[type="number"]').forEach(input => {
    input.addEventListener('change', function () {
        const productId = this.getAttribute('data-product-id');
        const newQuantity = this.value;

        if (newQuantity < 1) {
            alert('Quantity must be at least 1');
            this.value = 1;
            return;
        }

        // Gui yeu cau AJAX
        fetch(`/Cart/UpdateQuantity`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ productId: productId, quantity: newQuantity }),
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // load total, quality
                document.querySelector('.order-box .ml-auto.h5').innerText = `${data.grandTotal} VND`;
            } else {
                alert(data.message || 'Error updating quantity');
            }
        })
        .catch(error => console.error('Error:', error));
    });
});
