import LogoutLink from "../Components/LogoutLink.tsx";
import AuthorizeView, { AuthorizedUser } from "../Components/AuthorizeView.tsx";


function Home() {

    return (
        <AuthorizeView>
            <div className="d-flex flex-column align-items-center">
                <h1>Home Page</h1>
                <a href="/order">
                    <button className="mt-3">Order Page</button>
                </a>
                <span className="mt-3">
                    <LogoutLink>
                        Logout <AuthorizedUser value="email" />
                    </LogoutLink>
                </span>
            </div>
        </AuthorizeView>
    );
}

export default Home;