    import React from "react";
    import "../../css/chat.css";
    import {FaPhoneAlt, FaVideo, FaInfoCircle, FaImage, FaCamera, FaMicrophone, FaSmile, FaPaperPlane } from "react-icons/fa";


    const Chat = () => {
        return (
            <div className="chat">
                {/* Top bar */}
                <div className="top">
                    <div className="user">
                        <img
                            src="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT9ekUELxAsRyCKpamtHX0AjguEtr0qnoKNOOd_LOswRJMBCm0cEbHLnA3kMl_iy4mxxAw&usqp=CAU"
                            alt=""/>
                        <div className="texts">
                            <span>Username</span>
                            <p>Lorem ipsum dolor, sit amet.</p>
                        </div>
                    </div>
                    <div className="icons">
                        <FaPhoneAlt size={20}/>
                        <FaVideo size={20}/>
                        <FaInfoCircle size={20}/>
                    </div>

                </div>

                {/* Chat messages */}
                <div className="center">
                    {/* Tin nhắn người khác */}
                    <div className="message">
                        <div className="texts">
                            <p>Xin chào! Đây là tin nhắn từ người   .</p>
                            <span>2 minutes ago</span>
                        </div>
                    </div>

                    {/* Tin nhắn của mình */}
                    <div className="message own">
                        <div className="texts">
                            <p>Chào bạn! Đây là tin nhắn của mình.</p>
                            <span>1 minute ago</span>
                        </div>
                    </div>

                    {/* Tin nhắn có hình ảnh */}
                    <div className="message own">
                        <div className="texts">
                            <img
                                src="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSsgwoaBn9r7NCC26PoLvp20r3tGZqgk-Eb2Q&s"
                                alt=""/>
                            <span>Just now</span>
                        </div>
                    </div>
                </div>

                {/* Bottom input */}
                <div className="bottom">
                    <div className="bottom-icons">
                        <FaImage size={20}/>
                        <FaCamera size={20}/>
                        <FaMicrophone size={20}/>
                    </div>

                    <input type="text" placeholder="Type a message..."/>
                    <div className="emoji">
                        <FaSmile size={20}/>
                    </div>
                    <button className="sendButton">
                        <FaPaperPlane/>
                    </button>
                </div>
            </div>
        );
    };

    export default Chat;
