import { createContext, useContext } from "react";
import { User } from "./Types";

export const UserContext = createContext<User | undefined>(undefined);

export function useUserContext() {
    const user = useContext(UserContext);

    if (!user) {
        throw new Error("User context is undefined. Make sure you are using this hook within an AuthorizeView.");
    }

    return user 
}