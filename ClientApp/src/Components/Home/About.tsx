import React from 'react';
import { Section } from '../Section';

export const HomeAboutComponent: React.FC<{}> = () => {
    return (
        <Section name="about">
            <div className="card mb-3">
                <div className="row">
                    <div className="col-lg-8">
                        <img
                            alt=""
                            className="card-img-top"
                            src="https://scontent-atl3-1.xx.fbcdn.net/v/t1.0-9/90236177_193430868779280_7776700032199163904_o.jpg?_nc_cat=110&_nc_sid=da1649&_nc_ohc=0XY9K3fWbWEAX_Zbm8t&_nc_ht=scontent-atl3-1.xx&oh=9c586229368c7fd9c9bf7160e9547a8e&oe=5ED5A2C7"
                        />
                    </div>
                    <div className="col-lg-4">
                        <div className="card-body">
                            <h5 className="card-title">What this site is</h5>
                            <p className="card-text quoted">
                                Lorem ipsum dolor sit amet, consectetur
                                adipiscing elit, sed do eiusmod tempor
                                incididunt ut labore et dolore magna aliqua. Ac
                                turpis egestas integer eget. Sed pulvinar proin
                                gravida hendrerit lectus. Arcu dui vivamus arcu
                                felis bibendum ut tristique et egestas. Enim sed
                                faucibus turpis in eu. Faucibus ornare
                                suspendisse sed nisi lacus sed viverra tellus
                                in. In iaculis nunc sed augue lacus viverra
                                vitae congue. Tortor at auctor urna nunc id
                                cursus metus aliquam. Non blandit massa enim
                                nec. Habitant morbi tristique senectus et.
                                Tincidunt augue interdum velit euismod in
                                pellentesque massa placerat. Feugiat nisl
                                pretium fusce id. Est ultricies integer quis
                                auctor elit sed vulputate mi sit. A lacus
                                vestibulum sed arcu. Aliquam ut porttitor leo a
                                diam sollicitudin. Ut ornare lectus sit amet est
                                placerat in egestas erat.
                            </p>
                            <p className="card-text">
                                <small className="text-muted">
                                    - An Important Person
                                </small>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
            <div className="card-deck">
                <div className="card">
                    <img
                        className="card-img-top"
                        src="https://scontent-dfw5-1.xx.fbcdn.net/v/t1.0-9/92507963_199397401515960_710265510607978496_n.jpg?_nc_cat=103&_nc_sid=da1649&_nc_ohc=rO58e_67n1kAX-9ZUYr&_nc_ht=scontent-dfw5-1.xx&oh=094996668fa0eeaaf4871cbd03be97bd&oe=5ED44E32"
                        alt=""
                    />
                    <div className="card-body">
                        <h5 className="card-title">Card title</h5>
                        <p className="card-text quoted">
                            This card has supporting text below as a natural
                            lead-in to additional content.
                        </p>
                        <p className="card-text">
                            <small className="text-muted">
                                - someone important
                            </small>
                        </p>
                    </div>
                </div>
                <div className="card">
                    <img
                        className="card-img-top"
                        src="https://scontent-dfw5-2.xx.fbcdn.net/v/t1.0-9/75015765_10102536624581298_1297348481538588672_o.jpg?_nc_cat=104&_nc_sid=7aed08&_nc_ohc=Slgiw4xU_eQAX8ZviJS&_nc_ht=scontent-dfw5-2.xx&oh=f134ed4cf597d817f4104920f6cf272e&oe=5ED64751"
                        alt=""
                    />
                    <div className="card-body">
                        <h5 className="card-title">Card title</h5>
                        <p className="card-text quoted">
                            This card has supporting text below as a natural
                            lead-in to additional content.
                        </p>
                        <p className="card-text">
                            <small className="text-muted">
                                - someone else importatnt
                            </small>
                        </p>
                    </div>
                </div>
            </div>
        </Section>
    );
};
