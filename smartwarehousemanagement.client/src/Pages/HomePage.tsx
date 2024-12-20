import LogoutLink from "../Components/LogoutLink.tsx";
import AuthorizeView, { AuthorizedUser } from "../Components/AuthorizeView.tsx";

function HomePage() {

    return (
        <AuthorizeView>
            <div className="d-flex flex-column align-items-center">
                <h1>Home Page</h1>
                <a href="/item">
                    <button className="mt-3">Create Item Page</button>
                </a>
                <a href="/order">
                    <button className="mt-3">Order Page</button>
                </a>
                <a href="/shipping">
                    <button className="mt-3">Shipping Page</button>
                </a>
                <a href="/view/orders">
                    <button className="mt-3">View Orders Page</button>
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

export default HomePage;