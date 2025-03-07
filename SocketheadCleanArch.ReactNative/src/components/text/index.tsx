import React from 'react';
import { StyleSheet, Text as RNText } from 'react-native';
import { fontFamily as ff } from '../../constants'
import Colors from '../../colors';

const Text = ({ children, style = {}, numberOfLines = 0, onPress }: TextProps) => {

    const defaultFontFamily = ff.regular;
    const defaultFontColor = Colors.textColor;

    const { fontFamily = defaultFontFamily, color = defaultFontColor, ...rest } = style;
    const textProps = {
        ...rest,
        numberOfLines,
        style: {
            ...StyleSheet.flatten(style),
            fontFamily,
            color
        },
    };

    if (onPress) {
        textProps.onPress = onPress;
    }

    return <RNText {...textProps}>{children}</RNText>;
};

export default Text;

interface TextProps {
    children: any,
    // style: {
    //     color: string,
    //     fontFamily: string,
    //     rest: any
    // },
    style: any,
    numberOfLines: number,
    onPress: Function,

}