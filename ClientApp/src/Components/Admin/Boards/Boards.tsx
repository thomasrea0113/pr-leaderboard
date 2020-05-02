import React from 'react';
import { NavLink, Route, useRouteMatch } from 'react-router-dom';
import { NewBoardComponent } from './NewBoard';
import { NavProps } from '../../../pages/Board/NavProps';

export const BoardsNav: React.FC<NavProps> = ({ path }) => (
    <li className="nav-link dropdown">
        <NavLink
            className="nav-item dropdown-toggle"
            data-toggle="dropdown"
            to={path}
            role="button"
            aria-haspopup="true"
            aria-expanded="false"
        >
            Boards
        </NavLink>
        <div className="dropdown-menu">
            <NavLink className="dropdown-item" to={`${path}/New`}>
                New Board
            </NavLink>
        </div>
    </li>
);

export const BoardsAdminComponent: React.FC<{}> = () => {
    // gets the relative path of the parent route
    const { path } = useRouteMatch();
    return (
        <>
            <h2>Boards Admin</h2>
            <Route path={`${path}/New`} component={NewBoardComponent} />
        </>
    );
};
