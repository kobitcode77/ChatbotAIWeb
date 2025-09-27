import "../../css/userInfo.css";
import '@fortawesome/fontawesome-free/css/all.min.css';

const Userinfo = () => {
    // Dữ liệu giả để hiển thị
    const currentUser = {
        avatar: "https://dimensions.edu.vn/upload/2025/03/anh-dai-dien-facebook-mac-dinh-002.webp",
        username: "A Bao"
    };

    return (
        <div className='userInfo'>
            <div className="user">
                <img src={currentUser.avatar} alt="User avatar" />
                <h2>{currentUser.username}</h2>
            </div>
            <div className="icons">
                <i className="fas fa-ellipsis-h"></i>  {/* icon more */}
                <i className="fas fa-edit"></i>         {/* icon edit */}
            </div>
        </div>
    );
};

export default Userinfo;
