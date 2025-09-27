import React, { useState } from "react";
import "../../css/addUser.css";

const AddUser = () => {
    const [user, setUser] = useState({
        username: "Alice",
        avatar: "./avatar.png",
    });



    const handleAdd = () => {
        // Chỉ giả lập thêm user
        console.log("User added!");
    };

    return (
        <div className="addUser">
            {/* Form tìm kiếm */}
            <form >
                <input type="text" placeholder="Username" name="username" />
                <button type="submit">Search</button>
            </form>

            {/* Hiển thị user tìm thấy */}
            {user && (
                <div className="user">
                    <div className="detail">
                        <img src={user.avatar} alt="" />
                        <span>{user.username}</span>
                    </div>
                    <button onClick={handleAdd}>Add User</button>
                </div>
            )}
        </div>
    );
};

export default AddUser;
