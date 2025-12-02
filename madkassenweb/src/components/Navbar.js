import React, { useState } from "react";
import { Link } from "react-router-dom";
import { AppBar, Toolbar, Button, Menu, MenuItem } from "@mui/material";
import thumbnailmad from "../images/thumbnailmad.png";
import useAuth from "../Hooks/useAuth";
import { useCart } from "../context/CartContext";
import { Toaster } from "react-hot-toast";
import userImage from "../assets/user.png";

const Navbar = () => {
    const { isAuthenticated, logout, getUserInfo } = useAuth();
    const { cartItems } = useCart();
    const [anchorEl, setAnchorEl] = useState(null);

    const totalItems = cartItems.reduce((total, item) => total + item.quantity, 0);
    const userInfo = getUserInfo();
    const userName = userInfo?.userName;

    const handleMenuOpen = (event) => {
        setAnchorEl(event.currentTarget);
    };

    const handleMenuClose = () => {
        setAnchorEl(null);
    };

    return (
        <AppBar position="fixed" className="bg-indigo-600 shadow-md">
            <Toolbar className="flex justify-between items-center px-6 py-3">
                <Link to="/">
                    <img
                        src={thumbnailmad}
                        alt="Madkassen Thumbnail"
                        className="h-10 cursor-pointer"
                    />
                </Link>

                {/* Middle Section: Navigation Tabs */}
                <div className="flex-1 flex justify-center space-x-6">
                    <Button
                        color="inherit"
                        component={Link}
                        to="/productlist"
                        className="text-white hover:bg-indigo-700 rounded-md px-4 py-2"
                    >
                        Produkter
                    </Button>
                    <Button
                        color="inherit"
                        component={Link}
                        to="/about"
                        className="text-white hover:bg-indigo-700 rounded-md px-4 py-2"
                    >
                        Om os
                    </Button>
                    {isAuthenticated() && (
                        <Button
                            color="inherit"
                            component={Link}
                            to="/profile"
                            className="text-white hover:bg-indigo-700 rounded-md px-4 py-2"
                        >
                            Min Profil
                        </Button>
                    )}
                </div>

                {/* Right Section: Cart and Profile */}
                <div className="flex items-center space-x-4 ml-auto">
                    {/* Cart */}
                    <Link to="/cart" className="relative">
                        <span className="text-white text-2xl">🛒</span>
                        {totalItems > 0 && (
                            <span className="absolute top-0 right-0 inline-flex items-center justify-center w-5 h-5 text-xs font-bold text-white bg-red-500 rounded-full -mt-1 -mr-1">
                {totalItems}
              </span>
                        )}
                    </Link>

                    {/* Profile */}
                    {isAuthenticated() ? (
                        <div className="flex items-center space-x-4">
                            <img
                                src={userImage}
                                alt="User"
                                className="h-8 w-8 rounded-full cursor-pointer"
                                onClick={handleMenuOpen}
                            />
                            <Button
                                color="inherit"
                                onClick={handleMenuOpen}
                                className="text-white hover:bg-indigo-700 rounded-md px-4 py-2"
                            >
                                {userName || "User"}
                            </Button>
                            <Menu anchorEl={anchorEl} open={Boolean(anchorEl)} onClose={handleMenuClose}>
                                <MenuItem onClick={handleMenuClose} component={Link} to="/profile">
                                    Profile
                                </MenuItem>
                                <MenuItem onClick={handleMenuClose} component={Link} to="/settings">
                                    Settings
                                </MenuItem>
                                <MenuItem
                                    onClick={() => {
                                        handleMenuClose();
                                        logout();
                                    }}
                                >
                                    Logout
                                </MenuItem>
                            </Menu>
                        </div>
                    ) : (
                        <Button
                            color="inherit"
                            component={Link}
                            to="/login"
                            className="text-white hover:bg-indigo-700 rounded-md px-4 py-2"
                        >
                            Login
                        </Button>
                    )}
                </div>
            </Toolbar>
            <Toaster position="top-center" reverseOrder={false} />
        </AppBar>
    );
};

export default Navbar;
