import React from 'react';
import { NavLink, Route, useRouteMatch } from 'react-router-dom';
import { NavProps } from '../../../pages/Board/NavProps';
import { ViewUsersComponent } from './ViewUsers';
import { DeleteUserComponent } from './DeleteUser';

export const UsersNav: React.FC<NavProps> = ({ path }) => (
    <li className="nav-link dropdown">
        <NavLink
            className="nav-item dropdown-toggle"
            data-toggle="dropdown"
            to={path}
            role="button"
            aria-haspopup="true"
            aria-expanded="false"
        >
            Users
        </NavLink>
        <div className="dropdown-menu">
            <NavLink className="dropdown-item" to={path}>
                All Users
            </NavLink>
        </div>
    </li>
);

export const UsersAdminComponent: React.FC<{}> = () => {
    // gets the relative path of the parent route
    const { path } = useRouteMatch();
    return (
        <>
            <h2>Users Admin</h2>
            <Route path={`${path}/Delete`} component={DeleteUserComponent} />
            <Route exact path={path} component={ViewUsersComponent} />
        </>
    );
};
