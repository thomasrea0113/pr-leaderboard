import React from 'react';
import { Row } from 'react-table';
import UserView from '../serverTypes/UserView';
import { Expander } from './StyleComponents';

const DivisionComponent: React.FC<{
    row: Row<UserView>;
}> = ({ row }) => {
    return (
        <div className="card" {...row.getRowProps()}>
            <img className="card-img-top" src="..." alt="Card ap" />
            <div className="card-body">
                <div className="card-title">
                    {Expander<UserView>()({ row })} View Boards
                </div>
                <p className="card-text">
                    {row.cells.map(cell => (
                        <span
                            key={row.id + cell.column.id}
                            className="col-sm-4"
                        >
                            {cell.column.Header} - {cell.render('Cell')}
                        </span>
                    ))}
                </p>
                <a href="?" className="btn btn-primary">
                    Go somewhere
                </a>
            </div>
        </div>
    );
};

export default DivisionComponent;
