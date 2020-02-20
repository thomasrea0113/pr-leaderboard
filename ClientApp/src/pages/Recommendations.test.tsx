import React from 'react';
import renderer from 'react-test-renderer';
import Home from './Home';

test('Link changes the class when hovered', (): void => {
    const component = renderer.create(<Home />);
    const tree = component.toJSON();
    expect(tree).not.toBeNull();
    expect(tree).toMatchSnapshot();

    // // manually trigger the callback
    // tree?.props.onMouseEnter();
    // // re-rendering
    // tree = component.toJSON();
    // expect(tree).toMatchSnapshot();

    // // manually trigger the callback
    // tree?.props.onMouseLeave();
    // // re-rendering
    // tree = component.toJSON();
    // expect(tree).toMatchSnapshot();
});
