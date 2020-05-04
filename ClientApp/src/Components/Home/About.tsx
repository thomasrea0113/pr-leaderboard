import React from 'react';

export const HomeAboutComponent: React.FC<{}> = () => {
    return (
        <div className="container">
            <div className="row">
                <div className="col-sm">
                    <div className="card mb-3">
                        <div className="embed-responsive embed-responsive-16by9">
                            <img
                                alt=""
                                className="card-img-top embed-responsive-item"
                                src="https://scontent-atl3-1.xx.fbcdn.net/v/t1.0-9/90236177_193430868779280_7776700032199163904_o.jpg?_nc_cat=110&_nc_sid=da1649&_nc_ohc=0XY9K3fWbWEAX_Zbm8t&_nc_ht=scontent-atl3-1.xx&oh=9c586229368c7fd9c9bf7160e9547a8e&oe=5ED5A2C7"
                            />
                        </div>
                        <div className="card-body">
                            <h5 className="card-title">Card title</h5>
                            <p className="card-text">
                                This is a wider card with supporting text below
                                as a natural lead-in to additional content. This
                                content is a little bit longer.
                            </p>
                            <p className="card-text">
                                <small className="text-muted">
                                    Last updated 3 mins ago
                                </small>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};
