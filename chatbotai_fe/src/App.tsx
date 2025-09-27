import React from 'react';
import List from "./components/home/List"
import './App.css';
import Chat from "./components/home/Chat";
import Notification from "./components/home/Notification";

const App = () => {


    return (
        <div className="container">
                <>
                    <List />
                    <Chat />
                </>

            <Notification/>
        </div>
    );
};

export default App;
