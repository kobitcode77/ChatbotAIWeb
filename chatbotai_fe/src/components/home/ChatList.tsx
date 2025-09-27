import React, { useState } from "react";
import "../../css/chatList.css";
import AddUser from "./AddUser";
import { FiSearch, FiPlus, FiMinus } from "react-icons/fi";

const ChatList: React.FC = () => {
    const [addMode, setAddMode] = useState(false);
    const [input, setInput] = useState("");

    const chats = [
        { chatId: "1", username: "Alice", avatar: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSuMURe3FxCxI7HEzvWWOtKQqZxsl7Rqe9jixVgdwV1jPdt-sHwHcIFM_gPsD_xL6etev8&usqp=CAU", lastMessage: "Hello! How are you?", isSeen: false },
        { chatId: "2", username: "Bob", avatar: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSuMURe3FxCxI7HEzvWWOtKQqZxsl7Rqe9jixVgdwV1jPdt-sHwHcIFM_gPsD_xL6etev8&usqp=CAU", lastMessage: "Let's meet tomorrow.", isSeen: true },
        { chatId: "3", username: "Charlie", avatar: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSuMURe3FxCxI7HEzvWWOtKQqZxsl7Rqe9jixVgdwV1jPdt-sHwHcIFM_gPsD_xL6etev8&usqp=CAU", lastMessage: "Typing...", isSeen: false },
    ];

    const filteredChats = chats.filter((c) =>
        c.username.toLowerCase().includes(input.toLowerCase())
    );

    return (
        <div className="chatList">
            {/* Thanh tìm kiếm */}
            <div className="search">
                <div className="searchBar">
                    <FiSearch className="icon" />
                    <input
                        type="text"
                        placeholder="Search"
                        value={input}
                        onChange={(e) => setInput(e.target.value)}
                    />
                </div>
                <div className="add" onClick={() => setAddMode((prev) => !prev)}>
                    {addMode ? <FiMinus /> : <FiPlus />}
                </div>
            </div>

            {/* Danh sách chat */}
            {filteredChats.map((chat) => (
                <div
                    className="item"
                    key={chat.chatId}
                    style={{
                        backgroundColor: chat.isSeen ? "transparent" : "#3a5ad9",
                    }}
                >
                    <img src={chat.avatar} alt="" />
                    <div className="texts">
                        <span>{chat.username}</span>
                        <p>{chat.lastMessage}</p>
                    </div>
                </div>
            ))}

            {/* Giao diện thêm người */}
            {addMode && <AddUser />}
        </div>
    );
};

export default ChatList;
