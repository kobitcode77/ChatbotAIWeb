import ChatList from "./ChatList"
import "../../css/list.css"
import UserInfo from "./UserInfo"

const List = () => {
    return (
        <div className='list'>
            <UserInfo/>
            <ChatList/>
        </div>
    )
}

export default List