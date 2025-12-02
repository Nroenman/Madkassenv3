import axios from "axios";

const API_URL = "http://localhost:5092/api/Product";

const getAuthToken = () => {
    return localStorage.getItem("authToken");
};

export const addProduct = async (productData) => {
    const token = getAuthToken();

    try {
        const response = await axios.post(API_URL, productData, {
            headers: {
                Authorization: `Bearer ${token}`,
                "Content-Type": "application/json",
            },
        });

        return response.data;
    } catch (error) {
        if (error.response) {
            console.error("Error adding product:", error);
            console.error("Response Data:", error.response.data);
            console.error("Response Status:", error.response.status);
        } else {
            console.error("Error adding product:", error);
        }

        // Re-throw so caller can handle it
        throw error;
    }
};
