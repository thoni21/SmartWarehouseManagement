import LogoutLink from "../Components/LogoutLink.tsx";
import AuthorizeView, { AuthorizedUser } from "../Components/AuthorizeView.tsx";

function Home() {
    return (
        <AuthorizeView>
            <h1>Home Page</h1>
            
            <span><LogoutLink>Logout <AuthorizedUser value="email" /></LogoutLink></span>
        </AuthorizeView>
    );
}

export default Home;